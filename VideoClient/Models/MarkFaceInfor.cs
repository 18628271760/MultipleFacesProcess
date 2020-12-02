namespace VideoClient.Models
{
    public class MarkFaceInfor
    {
        public int left;
        public int top;
        public int width;
        public int height;
        public string name;
        public string alive;
        public byte[] faceFeatureData;
        public int faceID;

        public MarkFaceInfor(int p1, int p2, int p3, int p4, string individul = null, string liveStatus = null, byte[] detectFeature = null, int p5 = -1)
        {
            left = p1;
            top = p2;
            width = p3;
            height = p4;
            name = individul;
            alive = liveStatus;
            faceFeatureData = detectFeature;
            faceID = p5;
        }
    }
}
