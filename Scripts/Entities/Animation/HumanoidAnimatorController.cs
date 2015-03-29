using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Entity))]
public class HumanoidAnimatorController : MonoBehaviour
{

    private Entity _entity;
    private Animator _animator;

    #region Animation Hashes
    private static int animSpeed = Animator.StringToHash("speed");
    private static int animDead = Animator.StringToHash("dead");
    #endregion

    private void Start()
    {
        _entity = GetComponent<Entity>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateAnimationValues();
    }

    private void UpdateAnimationValues()
    {
        switch (_entity.LivingState)
        {
            case EntityLivingState.Alive:
                _animator.SetBool(animDead, false);
                break;
            case EntityLivingState.Dead:
                _animator.SetBool(animDead, true);
                break;
        }

        _animator.SetFloat(animSpeed, _entity.CurrentSpeed);
    }
}
