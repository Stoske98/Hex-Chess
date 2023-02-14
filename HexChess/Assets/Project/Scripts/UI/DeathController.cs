using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathController : MonoBehaviour
{
    public Image[] light = new Image[8];
    public Image[] dark = new Image[8];
    public Color death_color;
    public List<Unit> light_dead_units = new List<Unit>();
    public List<Unit> dark_dead_units = new List<Unit>();

    public TextMeshProUGUI light_dead_counter_txt;
    public TextMeshProUGUI dark_dead_counter_txt;

    public Transform light_grall;
    public Transform dark_grall;

    [Header("Grall Fire Controll")]
    public int start_emission;
    private int emission;
    public int increase;
    public ParticleSystem particle;
    ParticleSystem.EmissionModule emissionModule;

    private void Start()
    {
        emissionModule = particle.emission;
        emission = start_emission;
        emissionModule.rateOverTime = emission;
    }
    public void IncreaseFire()
    {
        emission += increase;
        emissionModule.rateOverTime = emission;
    }
    public void DecreaseFire()
    {
        emission -= increase;
        emissionModule.rateOverTime = emission;
    }
    public void OnUnitDeath(Unit unit, bool force_death = true)
    {
        StartCoroutine(UnitDeath(unit,2,force_death));
    }
    public IEnumerator UnitDeath(Unit unit, float time, bool force_death = true)
    {
        if(!force_death)
        {
            yield return new WaitForSeconds(time);
            unit.game_object.SetActive(false);

        }else
            unit.game_object.SetActive(false);

        if (!unit.GetType().Equals(typeof(JesterLightIllusion)) && !unit.GetType().Equals(typeof(JesterDarkIllusion)))
        {
            bool paint = true;
            foreach (Unit u in GameManager.Instance.units)
            {
                if (u != unit)
                {
                    if (unit.class_type == u.class_type && unit.unit_type == u.unit_type)
                    {
                        if (u.current_state != u.death_state)
                        {
                            paint = false;
                            break;
                        }
                    }
                }
            }
            if (paint)
            {
                if (unit.class_type == ClassType.Light)
                    light[(int)unit.unit_type - 1].color = death_color;
                else
                    dark[(int)unit.unit_type - 1].color = death_color;
            }

            if (unit.class_type == ClassType.Light)
                light_dead_units.Add(unit);
            else
                dark_dead_units.Add(unit);

            light_dead_counter_txt.text = "x" + light_dead_units.Count.ToString();
            dark_dead_counter_txt.text = dark_dead_units.Count.ToString() + "x";

            TryToUpgradeKingSpecialUI();

            IncreaseFire();

        }
        
    }

    private void TryToUpgradeKingSpecialUI()
    {
        if(light_dead_units.Count > 9)
        {
            light_grall.GetChild(0).gameObject.SetActive(true);
            light_grall.GetChild(1).gameObject.SetActive(true);
            light_grall.GetChild(2).gameObject.SetActive(true);
        }else if(light_dead_units.Count > 5)
        {
            light_grall.GetChild(0).gameObject.SetActive(true);
            light_grall.GetChild(1).gameObject.SetActive(true);
        }else if(light_dead_units.Count > 2)
            light_grall.GetChild(0).gameObject.SetActive(true);

        if (dark_dead_units.Count > 9)
        {
            dark_grall.GetChild(0).gameObject.SetActive(true);
            dark_grall.GetChild(1).gameObject.SetActive(true);
            dark_grall.GetChild(2).gameObject.SetActive(true);
        }
        else if (dark_dead_units.Count > 5)
        {
            dark_grall.GetChild(0).gameObject.SetActive(true);
            dark_grall.GetChild(1).gameObject.SetActive(true);
        }
        else if (dark_dead_units.Count > 2)
            dark_grall.GetChild(0).gameObject.SetActive(true);
    }
    public void OnReset()
    {
        foreach (Image light_image in light)
            light_image.color = Color.white;

        foreach (Image dark_image in dark)
            dark_image.color = Color.white;

        light_dead_units.Clear();
        dark_dead_units.Clear();

        light_dead_counter_txt.text = "x" + light_dead_units.Count.ToString();
        dark_dead_counter_txt.text = dark_dead_units.Count.ToString() + "x";

        foreach (Transform child in light_grall)
            child.gameObject.SetActive(false);

        foreach (Transform child in dark_grall)
            child.gameObject.SetActive(false);

        emission = start_emission;
        emissionModule.rateOverTime = emission;
    }
}
