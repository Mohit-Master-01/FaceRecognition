namespace FaceRecognition.Repositories.Interfaces
{
    public interface IFaceRecignition
    {
        float[] GetFaceEncoding(string imagePath);
        byte[] SerializeEncoding(float[] encoding);
        float[] DeserializeEncoding(byte[] bytes);
        double CompareEncodings(float[] encoding1, float[] encoding2);
        string StartCameraAndCapture();  // NEW: to launch live camera detection
    }
}
