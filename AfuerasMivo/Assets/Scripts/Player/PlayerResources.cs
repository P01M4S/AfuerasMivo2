using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class PlayerResources : MonoBehaviour
{

    //Components
    private Animator _animator;

    //Inputs
    private InputAction _healthPotionInput;
    private InputAction _manaPotionInput;
    
    //ManaBar
    [Header("Mana")]
    public float currentMana = 100;
    public float maxMana = 100;
    public float mana;
    public Image manaBarImage;
    [SerializeField] private int _manaReg = 25;

    //ManaHealth
    [Header("Health")]
    public float maxHealth = 100;
    public float currentHealth = 100;
    public Image healthBarImage;
    [SerializeField] private int _healthReg = 25;

    [Header("Texts")]
    public Text manaText;
    public Text healthText;
    public Text moneyText;

    //Potions
    [Header("Potions")]
    public int manaPotions = 0;
    public int healthPotions = 0; 

    //Money
    [Header("Money")]
    public int money = 0;
    public Text monetText;   

    //Player
    private PlayerAbility _playerAbility;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerAbility = GetComponent<PlayerAbility>();

        _healthPotionInput = InputSystem.actions["PotionsHealth"];
        _manaPotionInput = InputSystem.actions["PotionsMana"];
    }

    void Start()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
    }


    void Update()
    {
        if(GameManager.Instance._isDead || GameManager.Instance._isPaused) return;

        if(_manaPotionInput.WasPressedThisFrame() && manaPotions > 0)
        {
            Mana();
        }
        if(_healthPotionInput.WasPressedThisFrame() && healthPotions > 0)
        {
            Health();
        }
    }

    void Mana()
    {
        currentMana += _manaReg;
        manaPotions --;
        ManaText();
        UpdateManaBar();
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
    }
    void Health()
    {
        currentHealth += _healthReg;
        healthPotions --;
        HealthText();
        UpdateHealthBar();
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void UpdateManaBar()
    {
        float mana = currentMana / maxMana;
        manaBarImage.fillAmount = mana;
    }
    public void ManaText()
    {
        manaText.text = "x" + manaPotions.ToString();
    } 
    
    public void UpdateHealthBar()
    {
        float life = (float)currentHealth / maxHealth;
        healthBarImage.fillAmount = life;
    }

    public void HealthText()
    {
        healthText.text = "x" + healthPotions.ToString();
    } 

    public void Money()
    {
        int valueRandom = Random.Range(239, 875);
        money += valueRandom;
        Debug.Log(valueRandom);
    }

    public void TakeDamage(float damage)
    {
        if(currentHealth <= 0) return;
        
        currentHealth -= damage;
        UpdateHealthBar();
        if(currentHealth <= 0)
        {
            GameManager.Instance._isDead = true;
            _animator.SetTrigger("IsDead");
            Debug.Log("Muerto");
            //Destroy(gameObject);
        }
    }
}