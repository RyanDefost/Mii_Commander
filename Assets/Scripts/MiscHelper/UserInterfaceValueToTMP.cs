using System;
using TMPro;
using UnityEngine;

public class UserInterfaceValueToTMP : MonoBehaviour
{
    [SerializeField] private Component valueHolder;
    [SerializeField] private string valueName;
    [SerializeField] private string prefix;
    [SerializeField] private string suffix;
    [SerializeField] private TMP_Text textComponent;

    private void OnValidate()
    {
        if (Application.isPlaying) return;

        if (!this.valueHolder || this.valueHolder is IUserInterfaceValueGetter) return;
        this.valueHolder = null;
        Debug.LogError($"{GetType()}-{GetInstanceID()}: toActivate needs to be {nameof(IUserInterfaceValueGetter)}");
        
        this.enabled = this.valueHolder;
    }

    private void Start()
    {
        ((IUserInterfaceValueGetter)this.valueHolder).SubscribeOnChangeValue(this.valueName, OnChanged);
        
        object result = ((IUserInterfaceValueGetter)this.valueHolder).GetValue(this.valueName, out Type returnType);
        OnChanged(result, returnType);
    }

    private void OnDestroy() => ((IUserInterfaceValueGetter)this.valueHolder).UnSubscribeOnChangeValue(this.valueName, OnChanged);

    private void OnChanged(object value, Type returnType)
    {
        if (!this.textComponent) return;

        if (value == null)
        {
            this.textComponent.text = $"{this.prefix}Null{this.suffix}";
            return;
        }

        string formattedValue = string.Empty;
        if (returnType == typeof(float) || value is float)
            formattedValue = ((float)value).ToString("F2");
        else if (returnType == typeof(int) || value is int)
            formattedValue = ((int)value).ToString();
        else if (returnType == typeof(bool) || value is bool)
            formattedValue = ((bool)value) ? "True" : "False";
        else
            formattedValue = value.ToString();

        this.textComponent.text = $"{this.prefix}{formattedValue}{this.suffix}";
    }
}