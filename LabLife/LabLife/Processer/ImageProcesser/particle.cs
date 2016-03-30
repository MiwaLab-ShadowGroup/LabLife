//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using OpenCvSharp.CPlusPlus;
//using OpenCvSharp;


//namespace LabLife.Processer.ImageProcesser
//{
//    public class particle : AImageProcesser
//    {



//        public override void ImageProcess(ref Mat src, ref Mat dst)
//        {

//            this.Update(ref src, ref dst);

//        }


//        static int PARTICLE_NUM = 5000;
//        static int BLOB_MAX = 20;

//        int particle_size;
//        int particle_size_u;
//        bool pattern;

//        double velocity_right; //右方向の速度
//        double velocity_left; //左方向の速度
//        double vel;
//        int radius;     //当たり判定のための領域半径
//        double gravity;
//        CvPoint vel2D;

//        int number;     //粒子量
//        int measuring;     //輪郭の量
//        int count;     //フレームカウント
//        int visible;
//        int visible_u;
//        int flag;
//        int count_thresh;
//        double r;
//        double g;
//        double b;
//        double r_u;
//        double g_u;
//        double b_u;
//        bool median;
//        bool same_color;
//        bool same_para;

//        int g_threth;
//        int v_threth;
//        double g_opt;
//        double v_opt;

//        int mode;


//        //画像
//        IplImage src_img;
//        IplImage opt_img;
//        IplImage gray_img;
//        IplImage count_gray_img;
//        IplImage shape_img;
//        IplImage temp_img;
//        IplImage dst_img;
//        IplImage shadow_img;
//        IplImage circle_img;
//        IplImage param;

//        //輪郭取得
//        //CvMemStorage *storage;
//        CvSeq contours;
//        CvSeq contour;
//        CvTreeNodeIterator it;
//        CvRect temp;
//        int j, n, k, l, blob_num, p;
//        double d_tmp;

//        CvRNG rng;
//        int nRng;

//        CvPoint point, tmp;
//        CvSize image_size;

//        CvSeq points;
//        CvPoint pt;

//        CvFont font;
//        //粒子パラメータ
//        struct p_data
//        {
//            double position_x;
//            double position_y;
//            double velocity_x;
//            double velocity_y;
//            double u_position_x;
//            double u_position_y;
//            double u_velocity_x;
//            double u_velocity_y;
//            int R;
//            int G;
//            int B;
//            int u_R;
//            int u_G;
//            int u_B;
//        };
//        p_data P;
//	    struct b_data
//        {
//            P particle[PARTICLE_NUM];
//            int particle_num;
//            double gravity;
//            double vel;
//            CvSeq contour;
//            CvRect rect;
//        };

//        b_data Blob;
        

//        double dx;
//        double dy;
//        double distance;


//        private void Update(ref Mat src, ref Mat dst)
//        {
           

//        }
//        public override string ToString()
//        {
//            return "particle";
//        }
//        public bool IsFirstFrame { get; private set; }

//    }
//}

