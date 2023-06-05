using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WhateverDevs.Core.Behaviours;

namespace ITCL.VisionNutricional.Runtime.Camera
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class FakeCloudResponse : WhateverBehaviour<FakeCloudResponse>
    {
        public CamTextureToCloudVision.AnnotateImageResponses SpaghettiResponse()
        {
            CamTextureToCloudVision.AnnotateImageResponse response = new();

            List<CamTextureToCloudVision.EntityAnnotation> labelAnnotations = new();

            CamTextureToCloudVision.EntityAnnotation label1 =
              new (description: "Food", mid: "/m/02wbm", score: 0.98121715f, topicality: 0.98121715f);
            labelAnnotations.Add(label1);
            
            CamTextureToCloudVision.EntityAnnotation label2 =
              new (description: "Noodle", mid: "/m/0mfnf", score: 0.8732241f, topicality: 0.8732241f);
            labelAnnotations.Add(label2);
            
            CamTextureToCloudVision.EntityAnnotation label3 =
              new (description: "Recipe", mid: "/m/0p57p", score: 0.8722756f, topicality: 0.8722756f);
            labelAnnotations.Add(label3);
            
            CamTextureToCloudVision.EntityAnnotation label4 =
              new (description: "Ingredient", mid: "/m/07xgrh", score: 0.86083114f, topicality: 0.86083114f);
            labelAnnotations.Add(label4);
            
            CamTextureToCloudVision.EntityAnnotation label5 =
              new (description: "Condiment", mid: "/m/0gtdm", score: 0.860352f, topicality: 0.860352f);
            labelAnnotations.Add(label5);
            
            CamTextureToCloudVision.EntityAnnotation label6 =
              new (description: "Rice noodles", mid: "/m/09jy30", score: 0.8289405f, topicality: 0.8289405f);
            labelAnnotations.Add(label6);
            
            CamTextureToCloudVision.EntityAnnotation label7 =
              new (description: "Rice noodles", mid: "/m/09jy30", score: 0.8289405f, topicality: 0.8289405f);
            labelAnnotations.Add(label7);
            
            CamTextureToCloudVision.EntityAnnotation label8 =
              new (description: "Staple food", mid: "/m/022tld", score: 0.82679814f, topicality: 0.82679814f);
            labelAnnotations.Add(label8);
            
            CamTextureToCloudVision.EntityAnnotation label9 =
              new (description: "Al dente", mid: "/m/04t8sg", score: 0.81479806f, topicality: 0.81479806f);
            labelAnnotations.Add(label9);
            
            CamTextureToCloudVision.EntityAnnotation label10 =
              new (description: "Bolognese sauce", mid: "/m/0416b0", score: 0.8039648f, topicality: 0.8039648f);
            labelAnnotations.Add(label10);
            
            CamTextureToCloudVision.EntityAnnotation label11 =
              new (description: "Pasta pomodoro", mid: "/m/04qsz7", score: 0.795f, topicality: 0.795f);
            labelAnnotations.Add(label11);
            
            CamTextureToCloudVision.EntityAnnotation label12 =
              new (description: "Chinese noodles", mid: "/m/02hz3p", score: 0.78556156f, topicality: 0.78556156f);
            labelAnnotations.Add(label12);
            
            CamTextureToCloudVision.EntityAnnotation label13 =
              new (description: "Hot dry noodles", mid: "/m/02fslx", score: 0.75453734f, topicality: 0.75453734f);
            labelAnnotations.Add(label13);
            
            CamTextureToCloudVision.EntityAnnotation label14 =
              new (description: "Spaghetti", mid: "/m/0772h", score: 0.7491398f, topicality: 0.7491398f);
            labelAnnotations.Add(label14);
            
            CamTextureToCloudVision.EntityAnnotation label15 =
              new (description: "Produce", mid: "/m/036qh8", score: 0.747802f, topicality: 0.747802f);
            labelAnnotations.Add(label15);
            
            CamTextureToCloudVision.EntityAnnotation label16 =
              new (description: "Dish", mid: "/m/02q08p0", score: 0.74460006f, topicality: 0.74460006f);
            labelAnnotations.Add(label16);
            
            CamTextureToCloudVision.EntityAnnotation label17 =
              new (description: "Cuisine", mid: "/m/01ykh", score: 0.7393332f, topicality: 0.7393332f);
            labelAnnotations.Add(label17);
            
            CamTextureToCloudVision.EntityAnnotation label18 =
              new (description: "Fried noodles", mid: "/m/09clx2", score: 0.72459495f, topicality: 0.72459495f);
            labelAnnotations.Add(label18);
            
            CamTextureToCloudVision.EntityAnnotation label19 =
              new (description: "Bigoli", mid: "/m/0263_br", score: 0.72205937f, topicality: 0.72205937f);
            labelAnnotations.Add(label19);
            
            CamTextureToCloudVision.EntityAnnotation label20 =
              new (description: "Tomate frito", mid: "/m/02vwbbb", score: 0.7153398f, topicality: 0.7153398f);
            labelAnnotations.Add(label20);
            
            CamTextureToCloudVision.EntityAnnotation label21 =
              new (description: "Meat", mid: "/m/04scj", score: 0.71496856f, topicality: 0.71496856f);
            labelAnnotations.Add(label21);
            
            CamTextureToCloudVision.EntityAnnotation label22 =
              new (description: "Stringozzi", mid: "/m/05f62b0", score: 0.6966022f, topicality: 0.6966022f);
            labelAnnotations.Add(label22);
            
            CamTextureToCloudVision.EntityAnnotation label23 =
              new (description: "Yi mein", mid: "/m/02z66r9", score: 0.68875325f, topicality: 0.68875325f);
            labelAnnotations.Add(label23);
            
            CamTextureToCloudVision.EntityAnnotation label24 =
              new (description: "Shirataki noodles", mid: "/m/025x6pj", score: 0.6848205f, topicality: 0.6848205f);
            labelAnnotations.Add(label24);
            
            CamTextureToCloudVision.EntityAnnotation label25 =
              new (description: "Capellini", mid: "/m/027rk7_", score: 0.6847338f, topicality: 0.6847338f);
            labelAnnotations.Add(label25);
            
            response.labelAnnotations = labelAnnotations;


            List<CamTextureToCloudVision.EntityAnnotation> localizedObjectAnnotations = new();

            CamTextureToCloudVision.Vertex vertex1_1 = new()
            {
              x = 0.24070789f,
              y = 0.011105332f
            };
            CamTextureToCloudVision.Vertex vertex1_2 = new()
            {
              x = 0.9843597f,
              y = 0.011105332f
            };
            CamTextureToCloudVision.Vertex vertex1_3 = new()
            {
              x = 0.9843597f,
              y = 0.8489234f
            };
            CamTextureToCloudVision.Vertex vertex1_4 = new()
            {
              x = 0.24070789f,
              y = 0.8489234f
            };
            List <CamTextureToCloudVision.Vertex> normalizedVertices1 = new (){ vertex1_1, vertex1_2, vertex1_3, vertex1_4 };

            CamTextureToCloudVision.BoundingPoly poly1 = new() { normalizedVertices = normalizedVertices1 };

            CamTextureToCloudVision.EntityAnnotation entity1 = new() { boundingPoly = poly1, mid = "/m/05z55", name = "Pasta", score = 0.83202004f };
            
            localizedObjectAnnotations.Add(entity1);
            
            
            CamTextureToCloudVision.Vertex vertex2_1 = new()
            {
              x = 0.0026485908f,
              y = 0.16759641f
            };
            CamTextureToCloudVision.Vertex vertex2_2 = new()
            {
              x = 0.24590372f,
              y = 0.16759641f
            };
            CamTextureToCloudVision.Vertex vertex2_3 = new()
            {
              x = 0.24590372f,
              y = 0.9940796f
            };
            CamTextureToCloudVision.Vertex vertex2_4 = new()
            {
              x = 0.0026485908f,
              y = 0.9940796f
            };
            List <CamTextureToCloudVision.Vertex> normalizedVertices2 = new (){ vertex2_1, vertex2_2, vertex2_3, vertex2_4 };

            CamTextureToCloudVision.BoundingPoly poly2 = new() { normalizedVertices = normalizedVertices2 };

            CamTextureToCloudVision.EntityAnnotation entity2 = new() { boundingPoly = poly2, mid = "/m/04brg2", name = "Tableware", score = 0.7514869f };
            
            localizedObjectAnnotations.Add(entity2);
            
            
            CamTextureToCloudVision.Vertex vertex3_1 = new()
            {
              x = 0.7222288f,
              y = 0.00038562613f
            };
            CamTextureToCloudVision.Vertex vertex3_2 = new()
            {
              x = 0.97016686f,
              y = 0.00038562613f
            };
            CamTextureToCloudVision.Vertex vertex3_3 = new()
            {
              x = 0.97016686f,
              y = 0.05853058f
            };
            CamTextureToCloudVision.Vertex vertex3_4 = new()
            {
              x = 0.7222288f,
              y = 0.05853058f
            };
            List <CamTextureToCloudVision.Vertex> normalizedVertices3 = new (){ vertex3_1, vertex3_2, vertex3_3, vertex3_4 };

            CamTextureToCloudVision.BoundingPoly poly3 = new() { normalizedVertices = normalizedVertices3 };

            CamTextureToCloudVision.EntityAnnotation entity3 = new() { boundingPoly = poly3, mid = "/m/04brg2", name = "Tableware", score = 0.56153077f };
            
            localizedObjectAnnotations.Add(entity3);
            
            
            CamTextureToCloudVision.Vertex vertex4_1 = new()
            {
              x = 0.87009406f,
              y = 0.0012050844f
            };
            CamTextureToCloudVision.Vertex vertex4_2 = new()
            {
              x = 0.9934719f,
              y = 0.0012050844f
            };
            CamTextureToCloudVision.Vertex vertex4_3 = new()
            {
              x = 0.9934719f,
              y = 0.111025065f
            };
            CamTextureToCloudVision.Vertex vertex4_4 = new()
            {
              x = 0.87009406f,
              y = 0.111025065f
            };
            List <CamTextureToCloudVision.Vertex> normalizedVertices4 = new (){ vertex4_1, vertex4_2, vertex4_3, vertex4_4 };

            CamTextureToCloudVision.BoundingPoly poly4 = new() { normalizedVertices = normalizedVertices4 };

            CamTextureToCloudVision.EntityAnnotation entity4 = new() { boundingPoly = poly4, mid = "/m/04brg2", name = "Tableware", score = 0.50383055f };
            
            localizedObjectAnnotations.Add(entity4);
            
            response.localizedObjectAnnotations = localizedObjectAnnotations;


            CamTextureToCloudVision.SafeSearchAnnotation safeSearchAnnotation = new()
            {
              adult = CamTextureToCloudVision.Likelihood.VERY_UNLIKELY,
              medical = CamTextureToCloudVision.Likelihood.VERY_UNLIKELY,
              racy = CamTextureToCloudVision.Likelihood.VERY_UNLIKELY,
              spoof = CamTextureToCloudVision.Likelihood.VERY_UNLIKELY,
              violence = CamTextureToCloudVision.Likelihood.VERY_UNLIKELY
            };
            
            response.safeSearchAnnotation = safeSearchAnnotation;

            
            CamTextureToCloudVision.AnnotateImageResponses responses = new();
            responses.responses.Add(response);
            return responses;
        }

        public CamTextureToCloudVision.AnnotateImageResponses SandwichResponse()
        {
          CamTextureToCloudVision.AnnotateImageResponse response = new();
          
          List<CamTextureToCloudVision.EntityAnnotation> labelAnnotations = new();
          
          CamTextureToCloudVision.EntityAnnotation label1 =
              new (description: "Food", mid: "/m/02wbm", score: 0.9756512f, topicality: 0.9756512f);
            labelAnnotations.Add(label1);
            
          CamTextureToCloudVision.EntityAnnotation label2 =
              new (description: "Sandwich", mid: "/m/0l515", score: 0.9156336f, topicality: 0.9156336f);
            labelAnnotations.Add(label2);
            
            CamTextureToCloudVision.EntityAnnotation label3 =
              new (description: "Bun", mid: "/m/0119x1zy", score: 0.892697f, topicality: 0.892697f);
            labelAnnotations.Add(label3);
            
            CamTextureToCloudVision.EntityAnnotation label4 =
              new (description: "Staple food", mid: "/m/022tld", score: 0.8783036f, topicality: 0.8783036f);
            labelAnnotations.Add(label4);
            
            CamTextureToCloudVision.EntityAnnotation label5 =
              new (description: "Recipe", mid: "/m/0p57p", score: 0.874066f, topicality: 0.874066f);
            labelAnnotations.Add(label5);
            
            CamTextureToCloudVision.EntityAnnotation label6 =
              new (description: "Fast food", mid: "/m/01_bhs", score: 0.8233275f, topicality: 0.8233275f);
            labelAnnotations.Add(label6);
            
            CamTextureToCloudVision.EntityAnnotation label7 =
              new (description: "Bread", mid: "/m/09728", score: 0.8106258f, topicality: 0.8106258f);
            labelAnnotations.Add(label7);
            
            CamTextureToCloudVision.EntityAnnotation label8 =
              new (description: "Ingredient", mid: "/m/07xgrh", score: 0.8014026f, topicality: 0.8014026f);
            labelAnnotations.Add(label8);
            
            CamTextureToCloudVision.EntityAnnotation label9 =
              new (description: "Baked goods", mid: "/m/052lwg6", score: 0.76503927f, topicality: 0.76503927f);
            labelAnnotations.Add(label9);
            
            CamTextureToCloudVision.EntityAnnotation label10 =
              new (description: "Melt sandwich", mid: "/m/04brsc", score: 0.76488227f, topicality: 0.76488227f);
            labelAnnotations.Add(label10);
            
            CamTextureToCloudVision.EntityAnnotation label11 =
              new (description: "Plate", mid: "/m/050gv4", score: 0.76090455f, topicality: 0.76090455f);
            labelAnnotations.Add(label11);
            
            CamTextureToCloudVision.EntityAnnotation label12 =
              new (description: "Poached egg", mid: "/m/0b0y41", score: 0.74769646f, topicality: 0.74769646f);
            labelAnnotations.Add(label12);
            
            CamTextureToCloudVision.EntityAnnotation label13 =
              new (description: "Meat", mid: "/m/04scj", score: 0.74013793f, topicality: 0.74013793f);
            labelAnnotations.Add(label13);
            
            CamTextureToCloudVision.EntityAnnotation label14 =
              new (description: "Produce", mid: "/m/036qh8", score: 0.73685825f, topicality: 0.73685825f);
            labelAnnotations.Add(label14);
            
            CamTextureToCloudVision.EntityAnnotation label15 =
              new (description: "Finger food", mid: "/m/0b3dyl", score: 0.72815716f, topicality: 0.72815716f);
            labelAnnotations.Add(label15);
            
            CamTextureToCloudVision.EntityAnnotation label16 =
              new (description: "White bread", mid: "/m/03dnxh", score: 0.7251898f, topicality: 0.7251898f);
            labelAnnotations.Add(label16);
            
            CamTextureToCloudVision.EntityAnnotation label17 =
              new (description: "Junk food", mid: "/m/0h55b", score: 0.7159931f, topicality: 0.7159931f);
            labelAnnotations.Add(label17);
            
            CamTextureToCloudVision.EntityAnnotation label18 =
              new (description: "Hamburger", mid: "/m/0cdn1", score: 0.7035606f, topicality: 0.7035606f);
            labelAnnotations.Add(label18);
            
            CamTextureToCloudVision.EntityAnnotation label19 =
              new (description: "Garnish", mid: "/m/06gpzn", score: 0.70272094f, topicality: 0.70272094f);
            labelAnnotations.Add(label19);
            
            CamTextureToCloudVision.EntityAnnotation label20 =
              new (description: "Comfort food", mid: "/m/04q6ng", score: 0.69962823f, topicality: 0.69962823f);
            labelAnnotations.Add(label20);
            
            CamTextureToCloudVision.EntityAnnotation label21 =
              new (description: "Seafood", mid: "/m/06nwz", score: 0.69461256f, topicality: 0.69461256f);
            labelAnnotations.Add(label21);
            
            CamTextureToCloudVision.EntityAnnotation label22 =
              new (description: "Fried egg", mid: "/m/01r22b", score: 0.68876034f, topicality: 0.68876034f);
            labelAnnotations.Add(label22);
            
            CamTextureToCloudVision.EntityAnnotation label23 =
              new (description: "Cheddar cheese", mid: "/m/01_r_", score: 0.6471708f, topicality: 0.6471708f);
            labelAnnotations.Add(label23);
            
            CamTextureToCloudVision.EntityAnnotation label24 =
              new (description: "Leaf vegetable", mid: "/m/05f725", score: 0.64022803f, topicality: 0.64022803f);
            labelAnnotations.Add(label24);
            
            CamTextureToCloudVision.EntityAnnotation label25 =
              new (description: "Delicacy", mid: "/m/0bh9q8s", score: 0.6386241f, topicality: 0.6386241f);
            labelAnnotations.Add(label25);
            
            response.labelAnnotations = labelAnnotations;
            
            
            List<CamTextureToCloudVision.EntityAnnotation> localizedObjectAnnotations = new();

            CamTextureToCloudVision.Vertex vertex1_1 = new()
            {
              x = 0.04034027f,
              y = 0.043389875f
            };
            CamTextureToCloudVision.Vertex vertex1_2 = new()
            {
              x = 0.8204004f,
              y = 0.043389875f
            };
            CamTextureToCloudVision.Vertex vertex1_3 = new()
            {
              x = 0.8204004f,
              y = 0.9950982f
            };
            CamTextureToCloudVision.Vertex vertex1_4 = new()
            {
              x = 0.04034027f,
              y = 0.9950982f
            };
            List <CamTextureToCloudVision.Vertex> normalizedVertices1 = new (){ vertex1_1, vertex1_2, vertex1_3, vertex1_4 };

            CamTextureToCloudVision.BoundingPoly poly1 = new() { normalizedVertices = normalizedVertices1 };

            CamTextureToCloudVision.EntityAnnotation entity1 = new() { boundingPoly = poly1, mid = "/m/02wbm", name = "Food", score = 0.80488527f };
            
            localizedObjectAnnotations.Add(entity1);
            
            response.localizedObjectAnnotations = localizedObjectAnnotations;

            List<CamTextureToCloudVision.AnnotateImageResponse> responseList = new() { response };
              
            CamTextureToCloudVision.AnnotateImageResponses responses = new();
            responses.responses = responseList;
            return responses;
        }
    }

}
