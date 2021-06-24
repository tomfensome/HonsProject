﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class StoreSimGui : MonoBehaviour
{
    public StoreSimulation storeSimulation;

    public Slider numShoppersSlider;
    [FormerlySerializedAs("numContagiousSlider")]
    public Slider numInfectiousSlider;
    public Slider maxTransmissionDistanceSlider;
    public Slider transmissionProbAtZeroDistanceSlider;
    public Slider transmissionProbAtMaxDistanceSlider;
    public Slider durationSlider;
    public Slider numberOfRegistersSlider;
    public Slider shopperMovementSpeedSlider;
    public Slider minTransactionTimeSlider;
    public Slider maxTransactionTimeSlider;
    public Slider timeScaleSlider;

    public TMP_Text numShoppersText;
    [FormerlySerializedAs("numContagiousText")]
    public TMP_Text numInfectiousText;
    public TMP_Text maxTransmissionDistanceText;
    public TMP_Text durationText;
    public TMP_Text transmissionProbAtZeroDistanceText;
    public TMP_Text transmissionProbAtMaxDistanceText;
    public TMP_Text transmissionProbAtMaxDistanceLabel;
    public TMP_Text numberOfRegistersText;
    public TMP_Text shopperMovementSpeedText;
    public TMP_Text minTransactionTimeText;
    public TMP_Text maxTransactionTimeText;
    public TMP_Text timeScaleText;

    public Toggle oneWayAislesToggle;
    public Toggle masksToggle;
    public Toggle vaccineToggle;

    public TMP_Text healthyCustomersText;
    public TMP_Text exposedCustomersText;
    public TMP_Text exposedPercentageText;
    public TMP_Text totalRuntimeText;

    float meterToFoot = 3.28084f;
    float footToMeter = 0.3048f;
    string transmissionProbAtMaxDistanceLabelText = "Exposure Probability at {0} ft";
    string healthyCustomerCountLabelText = "Number of Healthy People: {0}";
    string exposedCustomerCountLabelText = "Number of Exposed People: {0}";
    string exposedPercentageLabelText = "Percent Exposed: {0}%";
    string runtimeLabelText = "Running Time: {0} seconds";

    int healthyCount;
    int exposedCount;

    float secondsSinceStart = 0.0f;

    int numShoppers;
    int numInfectious;
    float maxTransmissionDistance;
    float exposureProbMinDistance;
    float exposureProbMaxDistance;
    int numberOfRegisters;
    bool oneWayAisles;
    bool masks;
    bool vaccine;
    float shopperMoveSpeed;
    float minTransactionTime;
    float maxTransactionTime;

    bool isPaused = false;
    float timeScale = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        storeSimulation.NumHealthyChanged += OnNumHealthyChanged;
        storeSimulation.NumContagiousChanged += NumExposedChanged;

        numShoppers = storeSimulation.DesiredNumShoppers;
        numInfectious = storeSimulation.DesiredNumInfectious;
        maxTransmissionDistance = storeSimulation.ExposureDistanceMeters * meterToFoot;
        exposureProbMinDistance = storeSimulation.ExposureProbabilityAtZeroDistance;
        exposureProbMaxDistance = storeSimulation.ExposureProbabilityAtMaxDistance;
        numberOfRegisters = storeSimulation.NumberOfCountersOpen;
        oneWayAisles = storeSimulation.OneWayAisles;
        masks = storeSimulation.Masks;
        vaccine = storeSimulation.Vaccine;
        shopperMoveSpeed = storeSimulation.ShopperSpeed;
        minTransactionTime = storeSimulation.MinPurchaseTime;
        maxTransactionTime = storeSimulation.MaxPurchaseTime;

        numShoppersSlider.value = storeSimulation.DesiredNumShoppers;
        numShoppersText.text = storeSimulation.DesiredNumShoppers.ToString();
        numInfectiousSlider.maxValue = storeSimulation.DesiredNumShoppers > 20 ? 20 : storeSimulation.DesiredNumShoppers;
        numInfectiousSlider.value = storeSimulation.DesiredNumInfectious;
        numInfectiousText.text = storeSimulation.DesiredNumInfectious.ToString();
        maxTransmissionDistanceSlider.value = storeSimulation.ExposureDistanceMeters * meterToFoot;
        maxTransmissionDistanceText.text = (storeSimulation.ExposureDistanceMeters * meterToFoot).ToString();
        transmissionProbAtZeroDistanceSlider.value = storeSimulation.ExposureProbabilityAtZeroDistance;
        transmissionProbAtZeroDistanceText.text = storeSimulation.ExposureProbabilityAtZeroDistance.ToString();
        transmissionProbAtMaxDistanceSlider.value = storeSimulation.ExposureProbabilityAtMaxDistance;
        transmissionProbAtMaxDistanceText.text = storeSimulation.ExposureProbabilityAtMaxDistance.ToString();
        transmissionProbAtMaxDistanceLabel.text = string.Format(transmissionProbAtMaxDistanceLabelText, maxTransmissionDistanceSlider.value);
        numberOfRegistersSlider.value = storeSimulation.NumberOfCountersOpen;
        numberOfRegistersText.text = storeSimulation.NumberOfCountersOpen.ToString();
        oneWayAislesToggle.isOn = storeSimulation.OneWayAisles;
        masksToggle.isOn = storeSimulation.Masks;
        vaccineToggle.isOn = storeSimulation.Vaccine;
        shopperMovementSpeedSlider.value = storeSimulation.ShopperSpeed;
        shopperMovementSpeedText.text = storeSimulation.ShopperSpeed.ToString("0.00");
        minTransactionTimeSlider.value = storeSimulation.MinPurchaseTime;
        minTransactionTimeSlider.minValue = 0.01f;
        minTransactionTimeSlider.maxValue = storeSimulation.MaxPurchaseTime - 0.1f;
        minTransactionTimeText.text = storeSimulation.MinPurchaseTime.ToString("0.00");
        maxTransactionTimeSlider.value = storeSimulation.MaxPurchaseTime;
        maxTransactionTimeSlider.minValue = storeSimulation.MinPurchaseTime;
        maxTransactionTimeSlider.maxValue = 10;
        maxTransactionTimeText.text = storeSimulation.MaxPurchaseTime.ToString("0.00");
        timeScaleSlider.value = Time.timeScale;
        //Format Sim Speed to whole number, affix x to signify multiplier.
        timeScaleText.text = Time.timeScale.ToString("0") + "x";
    }

    // Update is called once per frame
    void Update()
    {
        secondsSinceStart += Time.deltaTime;
        UpdateTimeText();
    }

    public void ResetSim()
    {
        secondsSinceStart = 0;
        healthyCount = 0;
        exposedCount = 0;
        storeSimulation.DesiredNumShoppers = numShoppers;
        storeSimulation.DesiredNumInfectious = numInfectious;
        storeSimulation.ExposureDistanceMeters = maxTransmissionDistance * footToMeter;
        storeSimulation.ExposureProbabilityAtZeroDistance = exposureProbMinDistance;
        storeSimulation.ExposureProbabilityAtMaxDistance = exposureProbMaxDistance;
        storeSimulation.NumberOfCountersOpen = numberOfRegisters;
        storeSimulation.OneWayAisles = oneWayAisles;
        storeSimulation.Masks = masks;
        storeSimulation.Vaccine = vaccine;
        storeSimulation.ShopperSpeed = shopperMoveSpeed;
        storeSimulation.MinPurchaseTime = minTransactionTime;
        storeSimulation.MaxPurchaseTime = maxTransactionTime;
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = timeScale;
        }
        UpdateExposurePercent();
        UpdateTimeText();
        storeSimulation.ResetSimulation();
        //SceneManager.LoadScene(0);

    }

    public void OnNumShoppersChanged()
    {
        numShoppers = (int)numShoppersSlider.value;
        numInfectiousSlider.maxValue = numShoppers > 20 ? 20 : numShoppers;
        numShoppersText.text = numShoppers.ToString();
    }

    public void OnNumInfectiousChanged()
    {
        numInfectious = (int)numInfectiousSlider.value;
        numInfectiousText.text = numInfectious.ToString();
    }

    public void OnMaxTransmissionDistanceChanged()
    {
        maxTransmissionDistance = maxTransmissionDistanceSlider.value;
        maxTransmissionDistanceText.text = maxTransmissionDistanceSlider.value.ToString("0.00");
        transmissionProbAtMaxDistanceLabel.text = string.Format(transmissionProbAtMaxDistanceLabelText, maxTransmissionDistanceSlider.value);
    }

    public void OnDurationChanged()
    {
        durationText.text = durationSlider.value.ToString("0") + "mins";
    }

    public void OnTransmissionProbablityAtMinDistanceChanged()
    {
        exposureProbMinDistance = transmissionProbAtZeroDistanceSlider.value;
        transmissionProbAtZeroDistanceText.text = exposureProbMinDistance.ToString("0.00");
        if (exposureProbMinDistance < transmissionProbAtMaxDistanceSlider.value)
        {
            transmissionProbAtMaxDistanceSlider.value = exposureProbMinDistance;
        }
        transmissionProbAtMaxDistanceSlider.maxValue = exposureProbMinDistance;
    }

    public void OnTransmissionProbablityAtMaxDistanceChanged()
    {
        exposureProbMaxDistance = transmissionProbAtMaxDistanceSlider.value;
        transmissionProbAtMaxDistanceText.text = exposureProbMaxDistance.ToString("0.00");
    }

    public void OnNumberOfRegistersChanged()
    {
        numberOfRegisters = (int)numberOfRegistersSlider.value;
        numberOfRegistersText.text = numberOfRegisters.ToString();
    }

    public void OnOneWayAislesToggleChanged(bool val)
    {
        oneWayAisles = oneWayAislesToggle.isOn;
    }

    public void OnMasksToggleChanged(bool val)
    {
        masks = masksToggle.isOn;
    }
    public void OnVaccineToggleChanged(bool val)
    {
        vaccine = vaccineToggle.isOn;
    }

    public void OnShopperMovementSpeedChanged()
    {
        shopperMoveSpeed = shopperMovementSpeedSlider.value;
        shopperMovementSpeedText.text = shopperMoveSpeed.ToString("0.00");
    }

    public void OnMinTransactionTimeChanged()
    {
        minTransactionTime = minTransactionTimeSlider.value;
        minTransactionTimeText.text = minTransactionTime.ToString("0.00");
        maxTransactionTimeSlider.minValue = minTransactionTime;
        if (maxTransactionTimeSlider.value < minTransactionTime)
        {
            maxTransactionTimeSlider.value = minTransactionTime;
        }
    }

    public void OnMaxTransactionTimeChanged()
    {
        maxTransactionTime = maxTransactionTimeSlider.value;
        maxTransactionTimeText.text = maxTransactionTime.ToString("0.00");
        if (maxTransactionTime > 0.1f)
        {
            minTransactionTimeSlider.maxValue = maxTransactionTime - 0.1f;
            if (minTransactionTimeSlider.value >= maxTransactionTime)
            {
                minTransactionTimeSlider.value = maxTransactionTime;
            }
        }
        else
        {
            minTransactionTimeSlider.maxValue = 0.1f;
            maxTransactionTimeSlider.minValue = 0.1f;
        }
    }

    public void OnNumHealthyChanged(int count)
    {
        healthyCustomersText.text = string.Format(healthyCustomerCountLabelText, count);
        healthyCount = count;
        UpdateExposurePercent();
    }

    public void NumExposedChanged(int count)
    {
        exposedCustomersText.text = string.Format(exposedCustomerCountLabelText, count);
        exposedCount = count;
        UpdateExposurePercent();
    }

    public void UpdateExposurePercent()
    {
        var exposedPercent = ((float)exposedCount / (float)(healthyCount + exposedCount)) * 100;
        if (exposedCount == 0 && healthyCount == 0)
        {
            exposedPercent = 0;
        }
        exposedPercentageText.text = string.Format(exposedPercentageLabelText, exposedPercent.ToString("0"));
    }

    public void UpdateTimeText()
    {
        totalRuntimeText.text = string.Format(runtimeLabelText, secondsSinceStart.ToString("0.00"));
    }

    public void OnUpdateTimeScale(float newTimeScale)
    {
        Time.timeScale = timeScaleSlider.value;
        //Format Sim Speed to whole number, affix x to signify multiplier.
        timeScaleText.text = timeScaleSlider.value.ToString("0") + "x";
    }

    public void OnMouseOverObject(string whichObj)
    {

    }

    public void Pause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            timeScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = timeScale;
        }
    }
}
