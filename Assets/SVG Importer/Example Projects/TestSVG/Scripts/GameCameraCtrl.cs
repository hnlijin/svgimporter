using System.Collections;
using UnityEngine;
 
[RequireComponent(typeof(Camera))]
public class GameCameraCtrl : MonoBehaviour
{
    Camera mCamera = null;
    float SCREEN_WIDTH = 1920f;//屏幕宽度
    float SCREEN_HEIGHT = 1080f; // 屏幕高度
    const float max_allow_width = 7.5f; // 最大允许滑动的宽度
    const float max_allow_height = 7.5f; // 最大允许滑动的高度

    float maxZoom = 7f;

    float minZoom = 1f;
 
    float distanceScale;

    Vector3 zoomTouchPos1;
    Vector3 zoomTouchPos2;
    bool zoomTouchPos1Flag = false;
    bool zoomTouchPos2Flag = false;

    bool touchBeg = false;
    bool slideMode = false;

    int logCount = 0;

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
        slideMode = true;

        // -------------- for mobile touch input start ----------------------
        if (Input.touchCount == 1)
        {
            // Debug.Log("touch slide -> 0: logCount = " + logCount++);
            var data = Input.GetTouch(0);
            zoomTouchPos1 = Vector3.zero;
            zoomTouchPos2 = Vector3.zero;
            distanceScale = 0f;
            zoomTouchPos1Flag = false;
            zoomTouchPos2Flag = false;

            if (data.phase == TouchPhase.Began) {
                // Debug.Log("touch slide -> 1: logCount = " + logCount++);
                StopAllCoroutines();
                // touch_beg = data.deltaPosition;
                // timeTouch = 0.000001f;
                // touchBeg = true;
                zoomTouchPos1Flag = true;
                zoomTouchPos1 = data.position;
                distanceScale = Vector3.Distance(zoomTouchPos1, zoomTouchPos2);

            } else if (data.phase == TouchPhase.Moved && touchBeg == true) {
                // Debug.Log("touch slide -> 2: logCount = " + logCount++ + ", dx = " + data.deltaPosition.x + ", dy =" + data.deltaPosition.y);
                // float scaleRate = SCREEN_HEIGHT / 2f / mCamera.orthographicSize;
                // this.Slide(data.deltaPosition / scaleRate, "touch0"); 
                
            } else if (data.phase == TouchPhase.Ended && touchBeg == true) {
                // Debug.Log("touch slide -> 3: logCount = " + logCount++);
                // touchBeg = false;
                // this.StartAutoSlide(touch_beg, data.position);

            }
        }
        else if (Input.touchCount == 2)
        {
            slideMode = false;
            // Debug.Log("touch zoom -> 1: logCount = " + logCount++);
            touchBeg = false;
            var d1 = Input.GetTouch(0);
            var d2 = Input.GetTouch(1);
            if (d1.phase == TouchPhase.Began && zoomTouchPos1Flag == false) {
                zoomTouchPos1Flag = true;
                zoomTouchPos1 = d1.position;
                distanceScale = Vector3.Distance(zoomTouchPos1, zoomTouchPos2);
                // Debug.Log("touch zoom -> 2: logCount = " + logCount++);
            }
            if (d2.phase == TouchPhase.Began) {
                zoomTouchPos2Flag = true;
                zoomTouchPos2 = d2.position;
                distanceScale = Vector3.Distance(zoomTouchPos1, zoomTouchPos2);
                // Debug.Log("touch zoom -> 3: logCount = " + logCount++);
            }
            // Debug.Log("touch zoom -> 4: d1.phase = " + d1.phase + ", d2.phase = " + d2.phase);
            // Debug.Log("touch zoom -> 4: zoomTouchPos1Flag = " + zoomTouchPos1Flag + ", zoomTouchPos2Flag = " + zoomTouchPos2Flag);
            if (zoomTouchPos1Flag == true && zoomTouchPos2Flag == true) {
                float dis = Vector3.Distance(d1.position, d2.position);
                // Debug.Log("touch zoom -> 5: dis = " + dis + ", dis2 = " + distanceScale);
                if (d1.phase == TouchPhase.Moved || d2.phase == TouchPhase.Moved) {
                    float scaleRate = SCREEN_HEIGHT / 2f / mCamera.orthographicSize;
                    float disDiff = (dis - distanceScale) / scaleRate;
                    // Debug.Log("touch zoom -> 6: diff = " + disDiff);
                    this.Zoom(disDiff * 10f);
                    this.Slide(0f, 0f);
                }
                distanceScale = dis;
            }
        } else {
            // touchBeg = false;
            zoomTouchPos1 = Vector3.zero;
            zoomTouchPos2 = Vector3.zero;
            distanceScale = 0f;
            zoomTouchPos1Flag = false;
            zoomTouchPos2Flag = false;
            // Debug.Log("touch reset.");
        }
        // -------------- for mobile touch input start end----------------------

        //--------------- for pc mouse input start ------------------
        if (slideMode == true) {
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
                    float scaleRate = SCREEN_HEIGHT / 2f / mCamera.orthographicSize;
                    this.Slide((pos - lastMousePos) / scaleRate, "mouse");
                    lastMousePos = pos;
                }
            } else {
                // if (timeTouch > 0.06f && lastMousePos != Vector2.zero) {
                //     Vector2 pos = Input.mousePosition;
                //     Vector2 diff = lastMousePos - pos;
                //     diff *= timeTouch * 50f;
                //     pos = lastMousePos + diff;
                //     this.StartAutoSlide(pos, Input.mousePosition);
                // }
                lastMousePos = Vector2.zero;
            }
            // zoom out
            if (Input.GetAxis("Mouse ScrollWheel") < 0) {
                Debug.Log("Time.deltaTime = " + Time.deltaTime);
                this.Zoom(Time.deltaTime * 50f);
                this.Slide(0f, 0f);
            }
            // Zoom in
            if (Input.GetAxis("Mouse ScrollWheel") > 0) {
                this.Zoom(-Time.deltaTime * 50f);
                this.Slide(0f, 0f);
            }
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
        float scaleRate = SCREEN_HEIGHT / 2f / mCamera.orthographicSize;
        StartCoroutine(RunSliderAction((ended.x - orign.x) / scaleRate, (ended.y - orign.y) / scaleRate, Vector2.Distance(orign, ended) / timeTouch / scaleRate));
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
    void Slide(Vector2 dp, string from)
    {
        // Debug.Log("Slide: logCount = " + logCount++ + ", from = " + from);
        this.Slide(dp.x, dp.y);
    }
    //滑动接口，参数是偏移量
    void Slide(float dx, float dy)
    {
        // Debug.Log("Slide2: logCount = " + logCount++ + ", from = action");
        dx = -dx;
        dy = -dy;
        var pos = transform.position;
        pos.x += dx;
        pos.y += dy;
        transform.position = pos;
 
        // 实际宽度 unity单位大小 = 分辨率 / 摄像机size / 100（单位像素比）
        float real_unit_width = SCREEN_WIDTH / (SCREEN_HEIGHT / 2f / mCamera.orthographicSize); 
        float real_unit_height = mCamera.orthographicSize * 2f;
 
        //process edge
        float leftMinPosX = -(max_allow_width - real_unit_width / 2f);
        if (transform.position.x < leftMinPosX) {
            var posOld = transform.position;
            transform.position = new Vector3(leftMinPosX, posOld.y, posOld.z);
        }
        float rightMaxPosX = max_allow_width - real_unit_width / 2f;
        if (transform.position.x > rightMaxPosX) {
            var posOld = transform.position;
            transform.position = new Vector3(rightMaxPosX, posOld.y, posOld.z);
        }
        float topMinPosY = -(max_allow_height - real_unit_height / 2f);
        if (transform.position.y < topMinPosY) {
            var posOld = transform.position;
            transform.position = new Vector3(posOld.x, topMinPosY, posOld.z);
        }
        float bottomMaxPosY = max_allow_height - real_unit_height / 2f;
        if (transform.position.y > bottomMaxPosY) {
            var posOld = transform.position;
            transform.position = new Vector3(posOld.x, bottomMaxPosY, posOld.z);
        }
    }
    //放大-缩小接口
    void Zoom(float delta)
    {
        float orthographicSize = mCamera.orthographicSize + delta;
        if (orthographicSize > maxZoom) {
            orthographicSize = maxZoom;
        }
        if (orthographicSize < minZoom) {
            orthographicSize = minZoom;
        }
        mCamera.orthographicSize = orthographicSize;
    } 
}