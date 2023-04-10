using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Proxima
{
    internal delegate void PropertySetter(Component component, object value);
    internal delegate object PropertyGetter(Component component);
    internal delegate bool PropertyUpdater(Component component, ref object value);

    internal class ProximaComponentCommands
    {
        [Serializable]
        internal class PropertyInfo
        {
            public string Name;
            public string Type;
            public object Value;

            [NonSerialized]
            public PropertySetter Setter;

            [NonSerialized]
            public PropertyGetter Getter;

            [NonSerialized]
            public PropertyUpdater Updater;

            [NonSerialized]
            public Type PropertyType;
        }

        [Serializable]
        internal class ComponentInfo
        {
            public int Id;
            public string Name;
            public object Properties;
            public List<PropertyInfo> Props => (List<PropertyInfo>)Properties;

            [NonSerialized]
            public Component Component;

            [NonSerialized]
            public bool Temp;
        }

        [Serializable]
        internal class ComponentList
        {
            public object Components = new List<ComponentInfo>();
            public List<ComponentInfo> Comps => (List<ComponentInfo>)Components;
            public List<int> Destroyed = new List<int>();
        }

        private static Dictionary<int, ComponentStream> _goToStream;
        private static Dictionary<string, ComponentStream> _streams;
        private static Dictionary<int, ComponentInfo> _idToComponentInfo;

        private static List<ComponentInfo> _pool = new List<ComponentInfo>();
        private static readonly int _poolSize = 25;

        private static ComponentInfo GetFromPool(int id, string name, Component component, bool temp)
        {
            ComponentInfo info;
            if (_pool.Count > 0)
            {
                info = _pool[_pool.Count - 1];
                _pool.RemoveAt(_pool.Count - 1);
            }
            else
            {
                info = new ComponentInfo {
                    Properties = new List<PropertyInfo>()
                };
            }

            info.Id = id;
            info.Name = name;
            info.Component = component;
            info.Temp = temp;
            return info;
        }

        private static void ReturnToPool(ComponentInfo info)
        {
            if (_pool.Count >= _poolSize)
            {
                return;
            }

            info.Id = 0;
            info.Name = null;
            info.Component = null;
            info.Props.Clear();
            _pool.Add(info);
        }

        private static bool AreEqual(object lhs, object rhs)
        {
            if (lhs == null && rhs == null)
            {
                return true;
            }

            if (lhs == null || rhs == null)
            {
                return false;
            }

            if (lhs is IList && rhs is IList)
            {
                var l1 = (IList)lhs;
                var l2 = (IList)rhs;
                if (l1.Count != l2.Count)
                {
                    return false;
                }

                for (int i = 0; i < l1.Count; ++i)
                {
                    if (!AreEqual(l1[i], l2[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return lhs.Equals(rhs);
        }

        private static object Copy(object obj)
        {
            if (obj is Array)
            {
                return ProximaSerialization.CopyArray((Array)obj);
            }

            if (obj is IList)
            {
                return ProximaSerialization.CopyList((IList)obj);
            }

            return obj;
        }

        [ProximaInitialize]
        public static void Init()
        {
            _goToStream = new Dictionary<int, ComponentStream>();
            _streams = new Dictionary<string, ComponentStream>();
            _idToComponentInfo = new Dictionary<int, ComponentInfo>();
        }

        [ProximaTeardown]
        public static void Teardown()
        {
            foreach (var stream in _streams.Values)
            {
                stream.Cleanup();
            }

            _goToStream.Clear();
            _streams.Clear();
            _idToComponentInfo.Clear();
        }

        private class ComponentStream
        {
            public HashSet<string> ActiveStreamIds = new HashSet<string>();
            public HashSet<string> PendingStreamIds = new HashSet<string>();
            public int GameObjectId;

            private List<Component> _components = new List<Component>();
            private ComponentList _componentList = new ComponentList();
            private ComponentList _changeList = new ComponentList();
            private int _lastUpdateFrame;
            private int _lastUpdatedIndex;

            public ComponentList Update(string id)
            {
                UpdateChangeList();

                if (PendingStreamIds.Contains(id))
                {
                    PendingStreamIds.Remove(id);
                    ActiveStreamIds.Add(id);
                    return _componentList;
                }

                bool changed = _changeList.Comps.Count > 0 || _changeList.Destroyed.Count > 0;
                return changed ? _changeList : null;
            }

            public void Cleanup()
            {
                foreach (var ci in _componentList.Comps)
                {
                    _idToComponentInfo.Remove(ci.Id);
                    ReturnToPool(ci);
                }
            }

            private void UpdateChangeList()
            {
                if (_lastUpdateFrame == Time.frameCount)
                {
                    return;
                }

                _lastUpdateFrame = Time.frameCount;

                var cis = _changeList.Comps;
                for (int i = 0 ; i < cis.Count; i++)
                {
                    if (cis[i].Temp)
                    {
                        ReturnToPool(cis[i]);
                    }
                }

                _changeList.Comps.Clear();
                _changeList.Destroyed.Clear();

                if (!ProximaGameObjectCommands.IdToGameObject.TryGetValue(GameObjectId, out var go) || !go)
                {
                    return;
                }

                go.GetComponents(_components);
                UpdateDeletedComponents();

                if (_lastUpdatedIndex >= _components.Count)
                {
                    if (_lastUpdatedIndex < ProximaInspector.MaxComponentUpdateFrequency)
                    {
                        _lastUpdatedIndex++;
                        return;
                    }
                    else
                    {
                        _lastUpdatedIndex = 0;
                    }
                }

                UpdateComponentInfo(_components[_lastUpdatedIndex]);
                _lastUpdatedIndex++;
            }

            private void UpdateDeletedComponents()
            {
                for (int i = _componentList.Comps.Count - 1; i >= 0; i--)
                {
                    var ci = _componentList.Comps[i];
                    if (!ci.Component)
                    {
                        _idToComponentInfo.Remove(ci.Id);
                        _componentList.Comps.RemoveAt(i);
                        _changeList.Destroyed.Add(ci.Id);
                        ReturnToPool(ci);
                    }
                }
            }

            private void UpdateComponentInfo(Component component)
            {
                if (component.hideFlags.HasFlag(HideFlags.HideInInspector))
                {
                    return;
                }

                var id = component.GetInstanceID();
                if (!_idToComponentInfo.TryGetValue(id, out var ci))
                {
                    ci = GetFromPool(id, component.GetType().Name, component, false);
                    _componentList.Comps.Add(ci);
                    _idToComponentInfo.Add(component.GetInstanceID(), ci);
                    CreateComponentProperties(component, ci);
                    _changeList.Comps.Add(ci);
                }
                else
                {
                    UpdateProperties(ci);
                }
            }

            private bool CallUpdater(Component component, PropertyInfo property)
            {
                return property.Updater(component, ref property.Value);
            }

            private bool UpdatePropertyValue(Component component, PropertyInfo property, bool force)
            {
                if (force)
                {
                    property.Value = Copy(property.Getter(component));
                    return true;
                }
                else if (property.Updater != null)
                {
                    return CallUpdater(component, property);
                }
                else if (property.Getter != null)
                {
                    var value = property.Getter(component);
                    if (!AreEqual(value, property.Value))
                    {
                        property.Value = Copy(value);
                        return true;
                    }
                }

                return false;
            }

            private void UpdateProperties(ComponentInfo ci)
            {
                ComponentInfo clci = null;
                for (int i = 0; i < ci.Props.Count; i++)
                {
                    var property = ci.Props[i];
                    if (UpdatePropertyValue(ci.Component, property, false))
                    {
                        if (clci == null)
                        {
                            clci = GetFromPool(ci.Id, ci.Name, ci.Component, true);
                            _changeList.Comps.Add(clci);
                        }

                        clci.Props.Add(property);
                    }
                }
            }

            private void CreateComponentProperties(Component component, ComponentInfo ci)
            {
                ci.Properties = new List<PropertyInfo>();
                var type = component.GetType();

                if (ProximaReflection_Generated.Properties.TryGetValue(component.GetType().FullName, out var props))
                {
                    CreatePropertiesFromOverride(ci, props);
                }
                else if (component is MonoBehaviour)
                {
                    CreatePropertiesForMonoBehaviour(component, ci);
                }
                else
                {
                    CreatePropertiesForNativeObject(component, ci);
                }
            }

            private void CreatePropertiesFromOverride(ComponentInfo ci, PropertyInfo[] props)
            {
                foreach (var prop in props)
                {
                    AddProperty(ci, prop.Name, prop.PropertyType, prop.Setter, prop.Getter, prop.Updater);
                }
            }

            private void AddProperty(ComponentInfo ci, string name, Type type, PropertySetter setter, PropertyGetter getter, PropertyUpdater updater)
            {
                var propInfo = new PropertyInfo {
                    Name = name,
                    Type = ProximaSerialization.GetSerializedTypeName(type),
                    PropertyType = type,
                    Setter = setter,
                    Getter = getter,
                    Updater = updater
                };

                UpdatePropertyValue(ci.Component, propInfo, true);
                ci.Props.Add(propInfo);
            }

            private void AddProperty(ComponentInfo ci, string name, System.Reflection.PropertyInfo property, FieldInfo field)
            {
                PropertySetter setter = null;
                if (property != null && property.CanWrite)
                {
                    setter = property.SetValue;
                }
                else if (field != null)
                {
                    setter = field.SetValue;
                }

                PropertyGetter getter = null;
                if (property != null && property.CanRead)
                {
                    getter = property.GetValue;
                }
                else if (field != null)
                {
                    getter = field.GetValue;
                }

                AddProperty(ci, name, property != null ? property.PropertyType : field.FieldType, setter, getter, null);
            }

            private void AddEnabledProperty(ComponentInfo ci)
            {
                PropertySetter setter = (c, v) => ((MonoBehaviour)c).enabled = (bool)v;
                PropertyGetter getter = c => ((MonoBehaviour)c).enabled;
                PropertyUpdater updater = (Component o, ref object v) => { var x = ((MonoBehaviour)o).enabled; if (!x.Equals((System.Boolean)v)) { v = x; return true; } return false; };
                AddProperty(ci, "enabled", typeof(bool), setter, getter, updater);
            }

            private void CreatePropertiesForMonoBehaviour(Component component, ComponentInfo ci)
            {
                AddEnabledProperty(ci);
                var type = component.GetType();
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if ((field.IsPublic || field.GetCustomAttribute<SerializeField>() != null) &&
                        field.GetCustomAttribute<HideInInspector>() == null)
                    {
                        // If there's a custom editor, often just changing a field will have no effect.
                        // Search for a corresponding property we can use instead.
                        // https://docs.unity3d.com/Manual/VariablesAndTheInspector.html
                        var fieldWithoutPrefix =
                            field.Name.StartsWith("m_") ? field.Name.Substring(2) :
                            // field.Name.StartsWith("k") ? field.Name.Substring(1) : // This isn't true...
                            field.Name.StartsWith("_") ? field.Name.Substring(1) :
                            field.Name;

                        var fieldWithoutPrefixUpperCase = fieldWithoutPrefix[0].ToString().ToUpper() + fieldWithoutPrefix.Substring(1);
                        var property = type.GetProperties().FirstOrDefault(p => p.Name == fieldWithoutPrefix || p.Name == fieldWithoutPrefixUpperCase);
                        if (property != null && property.PropertyType == field.FieldType && ShouldUseProperty(property))
                        {
                            AddProperty(ci, property.Name, property, field);
                        }
                        else if (ShouldUseField(field))
                        {
                            AddProperty(ci, field.Name, null, field);
                        }
                    }
                }
            }

            private static HashSet<string> _propertySkipList = new HashSet<string>
            {
                "name", "tag", "hideFlags", "gameObject", "transform",
                "camera", "light", "meshFilter", "meshRenderer", "boxCollider", "scene",
                "material", "mesh"
            };

            private static HashSet<string> _supportedTypes = new HashSet<string>
            {
                typeof(string).Name, typeof(bool).Name, typeof(byte).Name, typeof(sbyte).Name,
                typeof(short).Name, typeof(ushort).Name,
                typeof(int).Name, typeof(uint).Name, typeof(long).Name, typeof(ulong).Name,
                typeof(float).Name, typeof(double).Name,
                typeof(Vector2).Name, typeof(Vector3).Name, typeof(Vector4).Name,
                typeof(Vector2Int).Name, typeof(Vector3Int).Name, typeof(Quaternion).Name,
                typeof(Rect).Name, typeof(RectInt).Name, typeof(Bounds).Name, typeof(BoundsInt).Name,
                typeof(Color).Name, typeof(LayerMask).Name
            };

            private static HashSet<string> _disallowedAttrs = new HashSet<string>
            {
                "HideInInspector", "ObsoleteAttribute", "NativeConditionalAttribute", "EditorBrowsableAttribute"
            };

            private bool IsSupportedType(Type type)
            {
                return _supportedTypes.Contains(type.Name) ||
                    type.IsEnum ||
                    (type.IsArray && IsSupportedType(type.GetElementType())) ||
                    (typeof(IList).IsAssignableFrom(type) && type.IsGenericType && IsSupportedType(type.GetGenericArguments()[0])) ||
                    type.IsSubclassOf(typeof(UnityEngine.Object));
            }

            private bool ShouldUse(IEnumerable<Attribute> attributes, string name, Type type)
            {
                var attrs = attributes.Select(a => a.GetType().Name);
                return
                    !_propertySkipList.Contains(name) &&
                    !attrs.Intersect(_disallowedAttrs).Any() &&
                    IsSupportedType(type);
            }

            private bool ShouldUseProperty(System.Reflection.PropertyInfo property)
            {
                return ShouldUse(property.GetCustomAttributes(), property.Name, property.PropertyType);
            }

            private bool ShouldUseField(FieldInfo field)
            {
                return ShouldUse(field.GetCustomAttributes(), field.Name, field.FieldType);
            }

            private void CreatePropertiesForNativeObject(Component component, ComponentInfo ci)
            {
                // Fallback to just enumerating public properties. This works well for some components.
                foreach (var property in component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (ShouldUseProperty(property))
                    {
                        AddProperty(ci, property.Name, property, null);
                    }
                }
            }
        }

        [ProximaStreamStart("Components")]
        public static void StartStream(string id, int gameObjectId)
        {
            if (!_goToStream.TryGetValue(gameObjectId, out var stream))
            {
                stream = new ComponentStream();
                stream.GameObjectId = gameObjectId;
                _goToStream.Add(gameObjectId, stream);
            }

            stream.PendingStreamIds.Add(id);
            _streams.Add(id, stream);
        }

        [ProximaStreamUpdate("Components")]
        public static ComponentList UpdateStream(string id)
        {
            if (_streams.TryGetValue(id, out var stream))
            {
                return stream.Update(id);
            }

            return null;
        }

        [ProximaStreamStop("Components")]
        public static void StopStream(string id)
        {
            if (_streams.TryGetValue(id, out var stream))
            {
                stream.PendingStreamIds.Remove(id);
                stream.ActiveStreamIds.Remove(id);
                if (stream.PendingStreamIds.Count == 0 && stream.ActiveStreamIds.Count == 0)
                {
                    stream.Cleanup();
                    _goToStream.Remove(stream.GameObjectId);
                }
            }

            _streams.Remove(id);
        }

        [ProximaCommand("Internal")]
        public static void SetProperty(int componentId, string name, string value)
        {
            if (!_idToComponentInfo.TryGetValue(componentId, out var ci))
            {
                Log.Error($"SetProperty: Component not found: {componentId}.");
                return;
            }

            var prop = ci.Props.Find(p => p.Name == name);
            if (prop == null)
            {
                Log.Error($"SetProperty: Property {name} not found on component {componentId}.");
                return;
            }

            if (prop.Setter != null)
            {
                if (!ProximaSerialization.TryDeserialize(prop.PropertyType, value, out var propValue))
                {
                    Log.Error($"SetProperty: Failed to deserialize {value} to {prop.PropertyType}.");
                    return;
                }

                Log.Verbose($"Set Property {prop.Name} to {propValue}.");
                prop.Value = Copy(propValue);
                prop.Setter(ci.Component, propValue);
            }
            else
            {
                Log.Error($"SetProperty: Property {name} on component {componentId} is not writable.");
            }
        }

        public static object GetPropertyValueForTest(int componentId, string name)
        {
            if (_idToComponentInfo.TryGetValue(componentId, out var ci))
            {
                var prop = ci.Props.Find(p => p.Name == name);
                if (prop != null)
                {
                    return prop.Getter(ci.Component);
                }
            }

            return null;
        }

        public static Type GetPropertyTypeForTest(int componentId, string name)
        {
            if (_idToComponentInfo.TryGetValue(componentId, out var ci))
            {
                var prop = ci.Props.Find(p => p.Name == name);
                if (prop != null)
                {
                    return prop.PropertyType;
                }
            }

            return null;
        }
    }
}