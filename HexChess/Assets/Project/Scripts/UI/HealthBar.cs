using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar
{
    public GameObject health_bar_prefab;
    public GameObject canvas_health_bar;
    public GameObject health_bar_gameo_object;
    public List<Image> HealthPoints;
    private Transform camera;

    public HealthBar(GameObject prefab)
    {
        health_bar_prefab = prefab;
        HealthPoints = new List<Image>();
        camera = GameManager.Instance.camera_controller.cam.transform;
    }
    public void Initialize(Unit unit)
    {
        canvas_health_bar = Object.Instantiate(health_bar_prefab);
        canvas_health_bar.transform.SetParent(unit.game_object.transform);
        health_bar_gameo_object = canvas_health_bar.transform.GetChild(0).gameObject;
        HealthPoints = new List<Image>();
        for (int i = 0; i < unit.stats.max_health; i++)
        {
            Image image = new GameObject().AddComponent<Image>();
            image.transform.SetParent(health_bar_gameo_object.transform.GetChild(0));
            image.transform.localScale = Vector3.one;
            image.transform.localPosition = Vector3.zero;
            HealthPoints.Add(image);
            image.color = Color.green;
        }
        canvas_health_bar.GetComponent<RectTransform>().transform.localPosition = Vector3.zero + Vector3.up * canvas_health_bar.GetComponent<RectTransform>().transform.localPosition.y;
        canvas_health_bar.SetActive(false);
    }

    public void Update(Unit unit)
    {
        HealthBarFiller(unit);
        ColorChanger(unit);
    }
    public void Activate()
    {
        //canvas_health_bar.transform.LookAt(camera);
        canvas_health_bar.transform.LookAt(canvas_health_bar.transform.position + camera.transform.forward);
        canvas_health_bar.SetActive(true);
    }

    public void Deactivate()
    {
        canvas_health_bar.SetActive(false);
    }

    public void HealthBarFiller(Unit unit)
    {
        for (int i = 0; i < HealthPoints.Count; i++)
        {
            HealthPoints[i].enabled = !DisplayHealthPoint(unit.stats.current_health, i);
        }
    }

    bool DisplayHealthPoint(float _health, int pointNumber)
    {
        return ((pointNumber) >= _health);
    }

    public void ColorChanger(Unit unit)
    {
        foreach (var image in HealthPoints)
        {
            Color healthColor = Color.Lerp(Color.red, Color.green, ((float)unit.stats.current_health / (float)unit.stats.max_health));
            image.color = healthColor;
        }
    }
}