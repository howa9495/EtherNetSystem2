using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    public float maxStamina =100f;
    private float _currentStamina;
    public float currentStamina { get { return _currentStamina; } set { _currentStamina = Mathf.Clamp(value,0,maxStamina); } }

    private void Start()
    {
        _currentStamina = maxStamina;
    }
    public void DecreaseStamina(float value, out bool success)
    {
        success = currentStamina >= value ? true : false;
        if (success)
        {
            currentStamina -= value;
        }
    }
    public void IncreaseStamina(float value)
    {
            currentStamina += value;
    }
    
    public float GetMaxStamina()
    {
        return maxStamina;
    }
    public float GetCurrentStamina()
    {
        return currentStamina;
    }
    public void ChangeMaxStamina(float changeValue, out float newValue)
    {
        maxStamina += changeValue;
        newValue = maxStamina;
    }
}
