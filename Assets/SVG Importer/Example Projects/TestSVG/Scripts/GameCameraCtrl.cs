﻿using System.Collections;
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

    Vector3 pos1;
    Vector3 pos2;

    const float SCALEPOSTION = 0.000001f; // 全局位置缩放大小
    bool touchBeg = false;

    void Start()
    {
        SCREEN_HEIGHT = Screen.height;
        SCREEN_WIDTH = Screen.width;
        mCamera = this.GetComponent<Camera>();
        if (mCamera == null) {
            Debug.LogError("need camera component");
        }
    }
 
    void Update()
    {
        if (touchBeg)
        {
            timeTouch += Time.deltaTime;
        }
        // -------------- for mobile touch input ----------------------
        if (Input.touchCount == 1)
        {
            var data = Input.GetTouch(0);
            this.Slide(data.deltaPosition * SCALEPOSTION);
            pos1 = Vector3.zero;
            pos2 = Vector3.zero;
            distanceScale = 0f;
            if (data.phase == TouchPhase.Began)
            {
                StopAllCoroutines();
                touch_beg = data.deltaPosition * SCALEPOSTION;
                timeTouch = 0.000001f;
                touchBeg = true;
            }
            if (data.phase == TouchPhase.Ended)
            {
                touchBeg = false;
                this.StartAutoSlide(touch_beg, data.position);
            }
        }
        else if (Input.touchCount == 2)
        {
            touchBeg = false;
            var d1 = Input.GetTouch(0);
            var d2 = Input.GetTouch(1);
            if (d1.phase == TouchPhase.Began)
            {
                pos1 = d1.position;
                distanceScale = Vector3.Distance(pos1, pos2);
            }
            if (d2.phase == TouchPhase.Began)
            {
                pos2 = d2.position;
                distanceScale = Vector3.Distance(pos1, pos2);
            }
            if (pos2 != Vector3.zero && pos2 != Vector3.zero)
            {
                float dis = Vector3.Distance(pos1, pos2);
                if (d1.phase == TouchPhase.Moved || d2.phase == TouchPhase.Moved)
                {
                    if (dis > distanceScale)
                    {
                        this.ZoomOut(Time.deltaTime * 100f);
                    }
                    else if (dis < distanceScale)
                    {
                        this.ZoomIn(Time.deltaTime * 100f);
                    }
                }
                distanceScale = dis;
            }
        }
        else
        {
            touchBeg = false;
            pos1 = Vector3.zero;
            pos2 = Vector3.zero;
            distanceScale = 0f;
        }
 
        //--------------- for pc mouse input ------------------
        //slide
        if (Input.GetMouseButton(0))
        {
            if (lastMousePos == Vector2.zero)
            {
                timeTouch = 0.000001f;
                StopAllCoroutines();
                lastMousePos = Input.mousePosition;
                touch_beg = lastMousePos;
                return;
            }
            else
            {
                timeTouch += Time.deltaTime;
                Vector2 pos = Input.mousePosition;
                this.Slide(0.015f * (pos - lastMousePos));
                lastMousePos = pos;
            }
        }
        else
        {
            if (lastMousePos != Vector2.zero) {
                this.StartAutoSlide(touch_beg, Input.mousePosition);
            }
            lastMousePos = Vector2.zero;
        }
        //----------------------for pc zoom
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Debug.Log("Time.deltaTime = " + Time.deltaTime);
            this.ZoomOut(Time.deltaTime * 100f);
            this.Slide(0f, 0f);
        }
        //Zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            this.ZoomIn(Time.deltaTime * 100f);
            this.Slide(0f, 0f);
        }
    }
 
    float maxtime = 3f;
    //开始惯性动画
    void StartAutoSlide(Vector2 orign, Vector2 ended)
    {
        if (Mathf.Abs(timeTouch) < 0.01f)
        {
            return;
        }
        maxtime = 3f;
        StopAllCoroutines();
        Debug.Log("slide map ,speed = " + (Vector2.Distance(orign, ended) / timeTouch / 1000f) + "  touchTime=" + timeTouch + " posended" + ended + "  beg" + orign);
 
        StartCoroutine(RunSliderAction(ended.x - orign.x, ended.y - orign.y, Vector2.Distance(orign, ended) / timeTouch / 1000f));
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
            speed -= Time.deltaTime * 5f;
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
 
        //实际宽度
        float real_width = mCamera.orthographicSize * 2 * SCREEN_WIDTH / SCREEN_HEIGHT;
        float real_height = real_width * SCREEN_HEIGHT / SCREEN_WIDTH;
 
        //process edge
        if (transform.position.x < -(max_allow_width - real_width / 2f)) {
            this.SetPosX(-max_allow_width);
        }
        if (transform.position.x > max_allow_width - real_width / 2f) {
            this.SetPosX(max_allow_width - real_width);
        }
        if (transform.position.y < -(max_allow_height - real_height / 2f)) {
            this.SetPosY(-max_allow_height);
        }
        if (transform.position.y > max_allow_height - real_height / 2f) {
            this.SetPosY(max_allow_height - real_height);
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
        float real_width = mCamera.orthographicSize * 2 * SCREEN_WIDTH / SCREEN_HEIGHT;
        float real_height = real_width * SCREEN_HEIGHT / SCREEN_WIDTH;
        var posOld = transform.position;
        transform.position = new Vector3(real_width / 2f + offsetX, posOld.y, posOld.z);
    }
    // 设置y偏移量，偏移量是从世界坐标原点开始计算
    void SetPosY(float offsetY)
    {
        float real_width = mCamera.orthographicSize * 2 * SCREEN_WIDTH / SCREEN_HEIGHT;
        float real_height = real_width * SCREEN_HEIGHT / SCREEN_WIDTH;
        var posOld = transform.position;
        transform.position = new Vector3(posOld.x, offsetY + real_height / 2f, posOld.z);
    }
 
}