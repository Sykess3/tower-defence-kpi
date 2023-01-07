using System.Collections;
using Core.Enemies;
using UnityEngine;

public abstract class EnemyView : MonoBehaviour
{
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    
    
    public bool IsInited { get; protected set; }

    protected Animator _animator;
    protected Enemy _enemy;

    protected const string DIED_KEY = "Died";

    public float SpeedFactor => _animator.speed;


    public virtual void Init(Enemy enemy)
    {
        _defaultColor = _skinnedMeshRenderer.material.color;
        _animator = GetComponent<Animator>();
        _healthBar.Initialize(enemy.Health);
        _enemy = enemy;
    }

    public virtual void Die()
    {
        _healthBar.Hide();
        _animator.SetBool(DIED_KEY, true);
    }

    public void SetSpeedFactor(float speedFactor)
    {
        _animator.speed = speedFactor;
    }

    public void OnSpawnAnimationFinished()
    {
        IsInited = true;
        GetComponent<TargetPoint>().IsEnabled = true;
    }

    private Coroutine _changeColorOnDamageCoroutine;
    private Color _defaultColor;

    public void UpdateHealthAmount(float healthAmount)
    {
        _healthBar.UpdateBar(healthAmount);
        if (_changeColorOnDamageCoroutine != null)
        { 
            StopCoroutine(_changeColorOnDamageCoroutine);
        }
        
        _changeColorOnDamageCoroutine = StartCoroutine(UpdateMeshColor());
        
    }

    private IEnumerator UpdateMeshColor()
    {
        _skinnedMeshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        _skinnedMeshRenderer.material.color = _defaultColor;
    }
}
