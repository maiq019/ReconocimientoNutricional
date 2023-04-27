﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using WhateverDevs.Core.Runtime.Ui;

// ReSharper disable InconsistentNaming

namespace ITCL.VisionNutricional.Runtime.Camera
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public class CamTextureToCloudVision : ActionOnButtonClick<CamTextureToCloudVision>
    {
        public delegate void CloudResponse(AnnotateImageResponses responses);

        public event CloudResponse OnCloudResponse;

        private const string URL = "https://vision.googleapis.com/v1/images:annotate?key=";
        [SerializeField] private string APIKey = "";
        [SerializeField] private FeatureType Feature_Type;
        [SerializeField] private int MaxResults = 20;

        //private Dictionary<string, string> headers;

        [System.Serializable]
        public class AnnotateImageRequests
        {
            public List<AnnotateImageRequest> requests;
        }

        [System.Serializable]
        public class AnnotateImageRequest
        {
            public Image image;
            public List<Feature> features;
        }

        [System.Serializable]
        public class Image
        {
            public string content;
        }

        [System.Serializable]
        public class Feature
        {
            public string type;
            public int maxResults;
        }

        [System.Serializable]
        public class ImageContext
        {
            public LatLongRect latLongRect;
            public List<string> languageHints;
        }

        [System.Serializable]
        public class LatLongRect
        {
            public LatLng minLatLng;
            public LatLng maxLatLng;
        }

        [System.Serializable]
        public class AnnotateImageResponses
        {
            public List<AnnotateImageResponse> responses;
        }

        [System.Serializable]
        public class AnnotateImageResponse
        {
            public List<FaceAnnotation> faceAnnotations;
            public List<EntityAnnotation> landmarkAnnotations;
            public List<EntityAnnotation> logoAnnotations;
            public List<EntityAnnotation> labelAnnotations;
            public List<EntityAnnotation> textAnnotations;
            public List<EntityAnnotation> localizedObjectAnnotations;
        }

        [System.Serializable]
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

        [System.Serializable]
        public class EntityAnnotation
        {
            public string mid;
            public string locale;
            public string description;
            public float score;
            public float confidence;
            public float topicality;
            public BoundingPoly boundingPoly;
            public List<LocationInfo> locations;
            public List<Property> properties;
        }

        [System.Serializable]
        public class BoundingPoly
        {
            public List<Vertex> normalizedVertices;
            public string name;
            public float score;
        }

        [System.Serializable]
        public class Landmark
        {
            public string type;
            public Position position;
        }

        [System.Serializable]
        public class Position
        {
            public float x;
            public float y;
            public float z;
        }

        [System.Serializable]
        public class Vertex
        {
            public float x;
            public float y;
        }

        [System.Serializable]
        public class entityRectVertices
        {
            public List<Vector2> entityRect = new List<Vector2>();
        }

        [System.Serializable]
        public class LocationInfo
        {
            private LatLng latLng;
        }

        [System.Serializable]
        public class LatLng
        {
            private float latitude;
            private float longitude;
        }

        [System.Serializable]
        public class Property
        {
            private string name;
            private string value;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public enum FeatureType
        {
            TYPE_UNSPECIFIED,
            FACE_DETECTION,
            LANDMARK_DETECTION,
            LOGO_DETECTION,
            LABEL_DETECTION,
            TEXT_DETECTION,
            SAFE_SEARCH_DETECTION,
            IMAGE_PROPERTIES
        }

        public enum LandmarkType
        {
            UNKNOWN_LANDMARK,
            LEFT_EYE,
            RIGHT_EYE,
            LEFT_OF_LEFT_EYEBROW,
            RIGHT_OF_LEFT_EYEBROW,
            LEFT_OF_RIGHT_EYEBROW,
            RIGHT_OF_RIGHT_EYEBROW,
            MIDPOINT_BETWEEN_EYES,
            NOSE_TIP,
            UPPER_LIP,
            LOWER_LIP,
            MOUTH_LEFT,
            MOUTH_RIGHT,
            MOUTH_CENTER,
            NOSE_BOTTOM_RIGHT,
            NOSE_BOTTOM_LEFT,
            NOSE_BOTTOM_CENTER,
            LEFT_EYE_TOP_BOUNDARY,
            LEFT_EYE_RIGHT_CORNER,
            LEFT_EYE_BOTTOM_BOUNDARY,
            LEFT_EYE_LEFT_CORNER,
            RIGHT_EYE_TOP_BOUNDARY,
            RIGHT_EYE_RIGHT_CORNER,
            RIGHT_EYE_BOTTOM_BOUNDARY,
            RIGHT_EYE_LEFT_CORNER,
            LEFT_EYEBROW_UPPER_MIDPOINT,
            RIGHT_EYEBROW_UPPER_MIDPOINT,
            LEFT_EAR_TRAGION,
            RIGHT_EAR_TRAGION,
            LEFT_EYE_PUPIL,
            RIGHT_EYE_PUPIL,
            FOREHEAD_GLABELLA,
            CHIN_GNATHION,
            CHIN_LEFT_GONION,
            CHIN_RIGHT_GONION
        };

        public enum Likelihood
        {
            UNKNOWN,
            VERY_UNLIKELY,
            UNLIKELY,
            POSSIBLE,
            LIKELY,
            VERY_LIKELY
        }

        protected override void ButtonClicked()
        {
            //headers = new Dictionary<string, string> { { "Content-Type", "application/json; charset=UTF-8" } };

            if (string.IsNullOrEmpty(APIKey))
                Logger.Error("No API key. Please set your API key into the \"Cam Texture To Cloud Vision(Script)\" component.");

            StartCoroutine(nameof(SendImageToCloudVisionCoroutine));
        }

        private IEnumerator SendImageToCloudVisionCoroutine(Texture2D texture2D)
        {
            if (APIKey == null) yield return null;

            byte[] jpg = texture2D.EncodeToJPG();
            string base64 = System.Convert.ToBase64String(jpg);

            AnnotateImageRequests requests = new AnnotateImageRequests { requests = new List<AnnotateImageRequest>() };

            AnnotateImageRequest request = new AnnotateImageRequest
            {
                image = new Image { content = base64 },
                features = new List<Feature>()
            };

            Feature feature = new Feature
            {
                type = Feature_Type.ToString(),
                maxResults = this.MaxResults
            };

            request.features.Add(feature);
            requests.requests.Add(request);

            string jsonData = JsonUtility.ToJson(requests, false);

            if (jsonData == string.Empty) yield return null;

            //byte[] postData = System.Text.Encoding.Default.GetBytes(jsonData);
            WWWForm postData = new WWWForm();
            postData.AddField("requests", jsonData);

            string url = URL + APIKey;
            //using(WWW www = new WWW(url, postData, headers)) {
            using UnityWebRequest www = UnityWebRequest.Post(url, postData);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Logger.Debug("CONNECTION ERROR " + www.error);
            }
            else
            {
                Logger.Debug(www.result.ToString().Replace("\n", "").Replace(" ", ""));
                AnnotateImageResponses responses = JsonUtility.FromJson<AnnotateImageResponses>(www.result.ToString());
                // SendMessage, BroadcastMessage or something like that.
                OnCloudResponse?.Invoke(responses);
            }

            yield return new WaitForEndOfFrame();
        }


        /// <summary>
        /// A sample implementation.
        /// </summary>
        private void Sample_OnAnnotateImageResponses(AnnotateImageResponses responses)
        {
            //Checks for lack of responses.
            if (responses.responses.Count <= 0) return;
            //Checks the label from the responses, this is the objects detected on the image.
            if (responses.responses[0].labelAnnotations is { Count: > 0 })
                //Prints the firs of the list, the one with most score.
                Logger.Debug("Description: " + responses.responses[0].labelAnnotations[0].description);
            
            //Checks the localized objects.
            if (responses.responses[0].localizedObjectAnnotations is not { Count: > 0 }) return;
            List<EntityAnnotation> localizedObjects = responses.responses[0].localizedObjectAnnotations;
            List<entityRectVertices> entityRects = new List<entityRectVertices>();
            foreach (EntityAnnotation obj in localizedObjects) //For each one, prints its name and vertices of its bounding box.
            {
                //if (entity.boundingPoly.name == AlgoEnLaBaseDeDatos)
                entityRectVertices entityRect = new entityRectVertices();
                entityRect.entityRect.AddRange(obj.boundingPoly.normalizedVertices.Select(vertice => new Vector2(vertice.x, vertice.y)));
                Logger.Debug(obj.boundingPoly.name + " detected at " + entityRect.entityRect[0] + ", " + entityRect.entityRect[1] + ", " +
                             entityRect.entityRect[2] + ", " + entityRect.entityRect[3] + ".");
                entityRects.Add(entityRect);
            }
        }
    }
}