using System;
using System.IO;
using Tensorflow;
using NumSharp;
using static Tensorflow.Binding;
using Newtonsoft.Json;
using System.Text;
using FaceRecognition.Repositories.Interfaces;
using System.Linq;
using OpenCvSharp;

namespace FaceRecognition.Repositories.Classes
{
    public class FaceRecognitionClass : IFaceRecignition
    {
        private readonly Graph _graph;
        private readonly Session _session;

        public FaceRecognitionClass()
        {
            _graph = new Graph().as_default();
            var graphDef = GraphDef.Parser.ParseFrom(File.ReadAllBytes("model/facenet.pb"));
            tf.import_graph_def(graphDef);
            _session = tf.Session(_graph);
        }

        public double CompareEncodings(float[] encoding1, float[] encoding2)
        {
            double sum = 0;
            for (int i = 0; i < encoding1.Length; i++)
                sum += Math.Pow(encoding1[i] - encoding2[i], 2);
            return Math.Sqrt(sum);
        }

        public float[] DeserializeEncoding(byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<float[]>(json);
        }

        public float[] GetFaceEncoding(string imagePath)
        {
            var imageTensor = LoadAndPreprocessImage(imagePath);

            var result = _session.run(
                fetches: new[] { _graph.OperationByName("InceptionResnetV1/Bottleneck/BatchNorm/cond/Merge").output },
                feed_dict: new FeedItem[]
                {
            new FeedItem(_graph.OperationByName("input").output, imageTensor),
            new FeedItem(_graph.OperationByName("phase_train").output, false)
                }
            );

            return result[0].ToArray<float>();
        }

        private NDArray LoadAndPreprocessImage(string imagePath)
        {
            // Load image using OpenCV
            Mat image = Cv2.ImRead(imagePath);
            Cv2.CvtColor(image, image, ColorConversionCodes.BGR2RGB);
            Cv2.Resize(image, image, new OpenCvSharp.Size(160, 160));

            // Convert to float32 and normalize to [0,1]
            var imageFloat = new Mat();
            image.ConvertTo(imageFloat, MatType.CV_32FC3, 1.0 / 255);

            // Allocate array for pixel data
            float[,,] data = new float[160, 160, 3];

            for (int y = 0; y < 160; y++)
            {
                for (int x = 0; x < 160; x++)
                {
                    var vec = imageFloat.At<Vec3f>(y, x);
                    data[y, x, 0] = vec.Item0; // R
                    data[y, x, 1] = vec.Item1; // G
                    data[y, x, 2] = vec.Item2; // B
                }
            }

            // Convert to NDArray
            var npImage = np.array(data, dtype: np.float32);

            // Add batch dimension → shape becomes [1,160,160,3]
            var batched = np.expand_dims(npImage, 0);

            return batched;
        }


        public byte[] SerializeEncoding(float[] encoding)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(encoding));
        }

        public string StartCameraAndCapture()
        {
            var capture = new VideoCapture(0);
            var window = new Window("Camera");
            var faceCascade = new CascadeClassifier("haarcascades/haarcascade_frontalface_default.xml");
            string faceFilePath = "captured_face.jpg";

            using var frame = new Mat();
            while (true)
            {
                capture.Read(frame);
                if (frame.Empty()) break;

                var gray = new Mat();
                Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);
                var faces = faceCascade.DetectMultiScale(gray, 1.1, 4);

                foreach (var rect in faces)
                {
                    Cv2.Rectangle(frame, rect, Scalar.Red, 2);
                    var faceROI = new Mat(frame, rect);
                    Cv2.ImWrite(faceFilePath, faceROI); // save cropped face
                }

                window.ShowImage(frame);

                int key = Cv2.WaitKey(30);
                if (key == 27) break; // ESC key
            }

            capture.Release();
            Cv2.DestroyAllWindows();
            return faceFilePath;
        }

    }
}
