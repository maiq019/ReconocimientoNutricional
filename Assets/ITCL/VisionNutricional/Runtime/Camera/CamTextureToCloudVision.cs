﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using ModestTree;
using UnityEngine;
using UnityEngine.Networking;
using WhateverDevs.Core.Behaviours;
using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.Localization.Runtime;
using Zenject;

// ReSharper disable InconsistentNaming

namespace ITCL.VisionNutricional.Runtime.Camera
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public class CamTextureToCloudVision : WhateverBehaviour<CamTextureToCloudVision>
    {
        public delegate void CloudResponse(AnnotateImageResponses responses);

        public static event CloudResponse OnCloudResponse;

        private const string URL = "https://vision.googleapis.com/v1/images:annotate?key=";
        public string APIKey = "";
        [SerializeField] private FeatureType Feature_Type;
        [SerializeField] private int MaxResults = 20;
        
        protected internal Texture2D capture;

        /// <summary>
        /// Reference to the localizer.
        /// </summary>
        [Inject] private ILocalizer localizer;

        //private Dictionary<string, string> headers;

        [Serializable]
        public class AnnotateImageRequests
        {
            public List<AnnotateImageRequest> requests;
        }

        [Serializable]
        public class AnnotateImageRequest
        {
            public Image image;
            public List<Feature> features;
        }

        [Serializable]
        public class Image
        {
            public string content;
        }

        [Serializable]
        public class Feature
        {
            public string type;
            public int maxResults;
        }

        [Serializable]
        public class AnnotateImageResponses
        {
            public List<AnnotateImageResponse> responses;
        }

        [Serializable]
        public class AnnotateImageResponse
        {
            public List<FaceAnnotation> faceAnnotations;
            public List<EntityAnnotation> landmarkAnnotations;
            public List<EntityAnnotation> labelAnnotations;
            public List<EntityAnnotation> localizedObjectAnnotations;
            public List<EntityAnnotation> logoAnnotations;
            public SafeSearchAnnotation safeSearchAnnotation;
            public List<EntityAnnotation> textAnnotations;
        }

        [Serializable]
        public class FaceAnnotation
        {
            public BoundingPoly boundingPoly;
            public BoundingPoly fdBoundingPoly;
            public List<Landmark> landmarks;
            public float rollAngle;
            public float panAngle;
            public float tiltAngle;
            public float detectionConfidence;
            public float landmarkingConfidence;
            public string joyLikelihood;
            public string sorrowLikelihood;
            public string angerLikelihood;
            public string surpriseLikelihood;
            public string underExposedLikelihood;
            public string blurredLikelihood;
            public string headwearLikelihood;
        }

        [Serializable]
        public class EntityAnnotation
        {
            public string mid;
            public string locale;
            public string name;
            public string description;
            public float score;
            public float confidence;
            public float topicality;
            public BoundingPoly boundingPoly;
            public List<LocationInfo> locations;
            public List<Property> properties;

            public EntityAnnotation()
            {
            }

            public EntityAnnotation(string description, string mid, float score, float topicality)
            {
                this.description = description;
                this.mid = mid;
                this.score = score;
                this.topicality = topicality;
            }
        }

        [Serializable]
        public class BoundingPoly
        {
            public List<Vertex> normalizedVertices;
            public string name;
            public float score;
        }
        
        [Serializable]
        public class Vertex
        {
            public float x;
            public float y;

            public Vertex()
            {
            }

            public Vertex(float x, float y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [Serializable]
        public class Landmark
        {
            public string type;
            public Position position;
        }

        [Serializable]
        public class Position
        {
            public float x;
            public float y;
            public float z;
        }

        [Serializable]
        public class SafeSearchAnnotation
        {
            public Likelihood adult;
            public Likelihood medical;
            public Likelihood racy;
            public Likelihood spoof;
            public Likelihood violence;
        }
        
        [Serializable]
        public class entityRectVertices
        {
            public List<Vector2> entityRect = new List<Vector2>();
        }

        [Serializable]
        public class LocationInfo
        {
            private LatLng latLng;
        }

        [Serializable]
        public class LatLng
        {
            private float latitude;
            private float longitude;
        }

        [Serializable]
        public class Property
        {
            private string name;
            private string value;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public enum FeatureType
        {
            TYPE_UNSPECIFIED,
            OBJECT_LOCALIZATION,
            FACE_DETECTION,
            LANDMARK_DETECTION,
            LOGO_DETECTION,
            LABEL_DETECTION,
            TEXT_DETECTION,
            SAFE_SEARCH_DETECTION,
            IMAGE_PROPERTIES
        }

        public enum Likelihood
        {
            UNKNOWN,
            VERY_UNLIKELY,
            UNLIKELY,
            POSSIBLE,
            LIKELY,
            VERY_LIKELY
        }
        
        protected internal void SendImageToCloudVision(Texture2D image)
        {
            if (APIKey.IsNullEmptyOrWhiteSpace()) Logger.Error(localizer["Debug/ApiKeyError"]); //Logger.Error(localizer["Debug/ApiKeyError"]);
            else
            {
                byte[] jpg = image.EncodeToJPG();
                string base64Image = Convert.ToBase64String(jpg);

                AnnotateImageRequests requests = new () { requests = new List<AnnotateImageRequest>() };

                AnnotateImageRequest request = new ()
                {
                    image = new Image { content = base64Image },
                    features = new List<Feature>()
                };

                Feature objectFeature = new()
                {
                    type = "OBJECT_LOCALIZATION",
                    maxResults = MaxResults
                };

                Feature feature = new ()
                {
                    type = Feature_Type.ToString(),
                    maxResults = MaxResults
                };

                request.features.Add(objectFeature);
                request.features.Add(feature);
                requests.requests.Add(request);

                string jsonData = JsonUtility.ToJson(requests, false);
                if (jsonData == string.Empty) return;
                
                System.IO.File.WriteAllText(Application.persistentDataPath+"Data.json", jsonData);
                
                StartCoroutine(SendImageToCloudVisionCoroutine(jsonData));
            }
        }

        private IEnumerator SendImageToCloudVisionCoroutine(string data)
        {
            if (APIKey.IsNullEmptyOrWhiteSpace()) yield return null;

            string url = URL + APIKey;

            UnityWebRequest www = new(url, UnityWebRequest.kHttpVerbPOST);
            
            www.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

            byte[] jsonBytes = Encoding.UTF8.GetBytes(data);

            www.uploadHandler = new UploadHandlerRaw(jsonBytes);

            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();
            
            Logger.Debug("Sending API web request");

            if (www.result != UnityWebRequest.Result.Success)
            {
                Logger.Error("API CONNECTION ERROR " + www.error);
            }
            else
            {
                Logger.Debug(www.result.ToString().Replace("\n", "").Replace(" ", ""));
                AnnotateImageResponses responses = JsonUtility.FromJson<AnnotateImageResponses>(www.downloadHandler.text);
                Logger.Debug("Responses translated");
                OnCloudResponse?.Invoke(responses);
            }
        }

        /// <summary>
        /// A sample implementation.
        /// </summary>
        [SuppressMessage("ReSharper", "CollectionNeverQueried.Local")]
        private void Sample_OnAnnotateImageResponses(AnnotateImageResponses responses)
        {
            //Checks for lack of responses.
            if (responses.responses.Count <= 0) return;
            //Checks the label from the responses, this is the objects detected on the image.
            if (responses.responses[0].labelAnnotations is { Count: > 0 })
                //Prints the firs of the list, the one with most score.
                //Logger.Debug("Description: " + responses.responses[0].labelAnnotations[0].description);
                Log.Debug("Description: " + responses.responses[0].labelAnnotations[0].description);
            
            //Checks the localized objects.
            if (responses.responses[0].localizedObjectAnnotations is not { Count: > 0 }) return;
            List<EntityAnnotation> localizedObjects = responses.responses[0].localizedObjectAnnotations;
            List<entityRectVertices> entityRects = new List<entityRectVertices>();
            foreach (EntityAnnotation obj in localizedObjects) //For each one, prints its name and vertices of its bounding box.
            {
                //if (entity.boundingPoly.name == AlgoEnLaBaseDeDatos)
                entityRectVertices entityRect = new entityRectVertices();
                entityRect.entityRect.AddRange(obj.boundingPoly.normalizedVertices.Select(vertice => new Vector2(vertice.x, vertice.y)));
                //Logger.Debug(obj.boundingPoly.name + " detected at " + entityRect.entityRect[0] + ", " + entityRect.entityRect[1] + ", " +
                             //entityRect.entityRect[2] + ", " + entityRect.entityRect[3] + ".");
                Log.Debug(obj.boundingPoly.name + " detected at " + entityRect.entityRect[0] + ", " + entityRect.entityRect[1] + ", " +
                          entityRect.entityRect[2] + ", " + entityRect.entityRect[3] + ".");
                entityRects.Add(entityRect);
            }
        }
    }
}
