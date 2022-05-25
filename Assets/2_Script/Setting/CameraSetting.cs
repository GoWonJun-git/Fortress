using UnityEngine;

namespace Percent.Utils
{
    /// <summary>
    /// 인 게임 카메라를 자동으로 설정해 주는 클래스입니다.
    /// 해상도를 입력하면 해당 해상도 영역은 무조건 카메라 안으로 들어오게 합니다.
    /// </summary>
    public class CameraSetting : MonoBehaviour {

        /// <summary>
        /// 시작 시 자동으로 설정되게 하고 싶을 경우 true
        /// </summary>
        [SerializeField] private bool isAwakeSetting = true;
        
        /// <summary>
        /// 설정 할 해상도의 Width 
        /// </summary>
        [SerializeField] private float gameWidth;

        /// <summary>
        /// 설정 할 해상도의 Height
        /// </summary>
        [SerializeField] private float gameHeight;

        /// <summary>
        /// 이미지들의 기본 베이스 unitSize
        /// </summary>
        [SerializeField] private int unitSize = 100;

        /// <summary>
        /// 게임의 메인 카메라. null일 경우 자동으로 카메라를 찾습니다.
        /// </summary>
        [SerializeField] private Camera mainCamera;

        private float cameraWidthSize;
        private float cameraHeightSize;

        void Start () 
        {
            if(mainCamera == null)
                mainCamera = Camera.main;

            if(isAwakeSetting)
                InitCamera( gameWidth, gameHeight, unitSize );
        }

        /// <summary>
        /// 카메라를 설정하는 함수입니다.
        /// </summary>
        /// <param name="width">설정 할 해상도의 Width</param>
        /// <param name="height">설정 할 해상도의 Height</param>
        /// <param name="unit">이미지들의 기본 베이스 unitSize</param>
        public void InitCamera( float width, float height, int unit )
        {
            gameWidth = width;
            gameHeight = height;
            unitSize = unit;

            float widthSize = gameWidth / unitSize;
            float heightSize = gameHeight / unitSize;
            float targetSize = 0;

            float perGame = gameWidth / gameHeight;
            float perScreen = (float)Screen.width / (float)Screen.height;

            if( perGame <= perScreen )
                targetSize = heightSize;
            else
                targetSize = widthSize / perScreen;
            
            mainCamera.orthographicSize = targetSize/2.0f;

            Vector3 cameraViewportToWorld = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0f));
            cameraWidthSize = cameraViewportToWorld.x;
            cameraHeightSize = cameraViewportToWorld.y;
        }

        #if UNITY_EDITOR
        private void Update() => InitCamera(gameWidth, gameHeight, unitSize);
        
        #endif
    }
}