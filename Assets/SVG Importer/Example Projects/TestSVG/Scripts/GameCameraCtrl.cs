using System.Collections;
using UnityEngine;
 
[RequireComponent(typeof(Camera))]
public class GameCameraCtrl : MonoBehaviour
{
    Camera mCamera = null;
    float SCREEN_WIDTH = 1920f;//屏幕宽度
    float SCREEN_HEIGHT = 1080f; // 屏幕高度
    const float max_allow_width = 75f; // 最大允许滑动的宽度
    const float max_allow_height = 75f; // 最大允许滑动的高度

    float maxZoom = 20f;

    float minZoom = 1f;
 
    float distanceScale;

    Vector3 zoomTouchPos1;
    Vector3 zoomTouchPos2;
    bool zoomTouchPos1Flag = false;
    bool zoomTouchPos2Flag = false;

    bool touchBeg = false;

    void Start()
    {
        SCREEN_HEIGHT = Screen.height;
        SCREEN_WIDTH = Screen.width;
        Input.multiTouchEnabled = true;
        mCamera = this.GetComponent<Camera>();
        if (mCamera == null) {
            Debug.LogError("need camera component");
        }
    }
 
    void Update()
    {
        if (touchBeg) {
            timeTouch += Time.deltaTime;
        }
        // -------------- for mobile touch input start ----------------------
        if (Input.touchCount == 1)
        {
            Debug.Log("touch slide - 0");
            var data = Input.GetTouch(0);
            this.Slide(data.deltaPosition / mCamera.orthographicSize / 100f);
            zoomTouchPos1 = Vector3.zero;
            zoomTouchPos2 = Vector3.zero;
            distanceScale = 0f;
            zoomTouchPos1Flag = false;
            zoomTouchPos2Flag = false;
            if (data.phase == TouchPhase.Began) {
                StopAllCoroutines();
                touch_beg = data.deltaPosition;
                timeTouch = 0.000001f;
                touchBeg = true;
                zoomTouchPos1Flag = true;
                zoomTouchPos1 = data.position;
                distanceScale = Vector3.Distance(zoomTouchPos1, zoomTouchPos2);
                Debug.Log("touch zoom - 0");
            }
            if (data.phase == TouchPhase.Ended) {
                touchBeg = false;
                this.StartAutoSlide(touch_beg, data.position);
            }
        }
        else if (Input.touchCount == 2)
        {
            Debug.Log("touch zoom - 1");
            touchBeg = false;
            var d1 = Input.GetTouch(0);
            var d2 = Input.GetTouch(1);
            if (d1.phase == TouchPhase.Began && zoomTouchPos1Flag == false) {
                zoomTouchPos1Flag = true;
                zoomTouchPos1 = d1.position;
                distanceScale = Vector3.Distance(zoomTouchPos1, zoomTouchPos2);
                Debug.Log("touch zoom - 2");
            }
            if (d2.phase == TouchPhase.Began) {
                zoomTouchPos2Flag = true;
                zoomTouchPos2 = d2.position;
                distanceScale = Vector3.Distance(zoomTouchPos1, zoomTouchPos2);
                Debug.Log("touch zoom - 3");
            }
            Debug.Log("touch zoom - 4" + ", d1.phase = " + d1.phase + ", d2.phase = " + d2.phase);
            Debug.Log("zoomTouchPos1Flag = " + zoomTouchPos1Flag + ", zoomTouchPos2Flag = " + zoomTouchPos2Flag);
            if (zoomTouchPos1Flag == true && zoomTouchPos2Flag == true) {
                float dis = Vector3.Distance(zoomTouchPos1, zoomTouchPos2);
                Debug.Log("touch zoom - 5" + "dis = " + dis + ", dis2 = " + distanceScale);
                if (d1.phase == TouchPhase.Moved || d2.phase == TouchPhase.Moved) {
                    if (dis > distanceScale) {
                        Debug.Log("touch zoom - 6");
                        this.ZoomOut(Time.deltaTime * 50f);
                    } else if (dis < distanceScale) {
                        Debug.Log("touch zoom - 7");
                        this.ZoomIn(Time.deltaTime * 50f);
                    }
                }
                distanceScale = dis;
            }
        } else {
            touchBeg = false;
            zoomTouchPos1 = Vector3.zero;
            zoomTouchPos2 = Vector3.zero;
            distanceScale = 0f;
            zoomTouchPos1Flag = false;
            zoomTouchPos2Flag = false;
            Debug.Log("touch reset.");
        }
        // -------------- for mobile touch input start end----------------------

        //--------------- for pc mouse input start ------------------
        // slide
        if (Input.GetMouseButton(0)) {
            if (lastMousePos == Vector2.zero) {
                timeTouch = 0.000001f;
                StopAllCoroutines();
                lastMousePos = Input.mousePosition;
                touch_beg = lastMousePos;
                return;
            } else {
                timeTouch += Time.deltaTime;
                Vector2 pos = Input.mousePosition;
                this.Slide((pos - lastMousePos) / mCamera.orthographicSize / 100f);
                lastMousePos = pos;
            }
        } else {
            if (lastMousePos != Vector2.zero) {
                this.StartAutoSlide(touch_beg, Input.mousePosition);
            }
            lastMousePos = Vector2.zero;
        }
        // zoom out
        if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            Debug.Log("Time.deltaTime = " + Time.deltaTime);
            this.ZoomOut(Time.deltaTime * 50f);
            this.Slide(0f, 0f);
        }
        // Zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            this.ZoomIn(Time.deltaTime * 50f);
            this.Slide(0f, 0f);
        }
        //--------------- for pc mouse input end ------------------
    }
 
    float maxtime = 3f;
    //开始惯性动画
    void StartAutoSlide(Vector2 orign, Vector2 ended)
    {
        if (Mathf.Abs(timeTouch) < 0.01f) {
            return;
        }

        maxtime = 3f;
        StopAllCoroutines();
        // Debug.Log("slide map: speed = " + (Vector2.Distance(orign, ended) / timeTouch / mCamera.orthographicSize / 100f));
        // Debug.Log("slide map: touchTime = " + timeTouch + ", posended = " + ended + ", posbeg = " + orign);
 
        StartCoroutine(RunSliderAction((ended.x - orign.x) / mCamera.orthographicSize / 100f, (ended.y - orign.y) / mCamera.orthographicSize / 100f, Vector2.Distance(orign, ended) / timeTouch / mCamera.orthographicSize / 100f));
        timeTouch = 0.000001f;
    }
    Vector2 touch_beg;
    float timeTouch = 0.000001f;
    IEnumerator RunSliderAction(float dx, float dy, float speed)
    {
        float time = 0f;
        float dis = 0f;
        while (time < maxtime && speed >= 0f)
        {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
            dis += Time.deltaTime * 0.01f;
            speed -= Time.deltaTime * 30f;
            // Debug.Log("speed = " + speed + ", dx = " + dx + ", dy = " + dy);
            this.Slide(Time.deltaTime * speed * dx, dy * Time.deltaTime * speed);
        }
    }
 
    Vector2 lastMousePos = Vector2.zero;
    //滑动接口，参数是偏移量
    void Slide(Vector2 dp)
    {
        this.Slide(dp.x, dp.y);
    }
    //滑动接口，参数是偏移量
    void Slide(float dx, float dy)
    {
        dx = -dx;
        dy = -dy;
        var pos = transform.position;
        pos.x += dx;
        pos.y += dy;
        transform.position = pos;
 
        // 实际宽度 unity单位大小 = 分辨率 / 摄像机size / 100（单位像素比）
        float real_unit_width = SCREEN_WIDTH / mCamera.orthographicSize / 100f;
        float real_unit_height = SCREEN_HEIGHT / mCamera.orthographicSize / 100f;

        // Debug.Log("real_unit_width = " + real_unit_width + ", real_unit_height = " + real_unit_height);
 
        //process edge
        float leftMinPosX = -(max_allow_width - real_unit_width / 2f);
        if (transform.position.x < leftMinPosX) {
            this.SetPosX(leftMinPosX);
        }
        float rightMaxPosX = max_allow_width - real_unit_width / 2f;
        if (transform.position.x > rightMaxPosX) {
            this.SetPosX(rightMaxPosX);
        }
        float topMinPosY = -(max_allow_height - real_unit_height / 2f);
        if (transform.position.y < topMinPosY) {
            this.SetPosY(topMinPosY);
        }
        float bottomMaxPosY = max_allow_height - real_unit_height / 2f;
        if (transform.position.y > bottomMaxPosY) {
            this.SetPosY(bottomMaxPosY);
        }
    }
    //放大接口
    void ZoomOut(float delta)
    {
        float orthographicSize = mCamera.orthographicSize + delta;
        if (orthographicSize > maxZoom) {
            orthographicSize = maxZoom;
        }
        mCamera.orthographicSize = orthographicSize;
    }
    //缩小接口
    void ZoomIn(float delta)
    {
        float orthographicSize = mCamera.orthographicSize - delta;
        if (orthographicSize <= minZoom) {
            orthographicSize = minZoom;
        }
        mCamera.orthographicSize = orthographicSize;
    }
    // 设置x偏移量，偏移量是从世界坐标原点开始计算
    void SetPosX(float offsetX)
    {
        // float real_width = mCamera.orthographicSize * 2 * SCREEN_WIDTH / SCREEN_HEIGHT;
        // float real_height = real_width * SCREEN_HEIGHT / SCREEN_WIDTH;
        // var posOld = transform.position;
        // transform.position = new Vector3(real_width / 2f + offsetX, posOld.y, posOld.z);
        var posOld = transform.position;
        transform.position = new Vector3(offsetX, posOld.y, posOld.z);
    }
    // 设置y偏移量，偏移量是从世界坐标原点开始计算
    void SetPosY(float offsetY)
    {
        // float real_width = mCamera.orthographicSize * 2 * SCREEN_WIDTH / SCREEN_HEIGHT;
        // float real_height = real_width * SCREEN_HEIGHT / SCREEN_WIDTH;
        // var posOld = transform.position;
        // transform.position = new Vector3(posOld.x, offsetY + real_height / 2f, posOld.z);
        var posOld = transform.position;
        transform.position = new Vector3(posOld.x, offsetY, posOld.z);
    }
 
}