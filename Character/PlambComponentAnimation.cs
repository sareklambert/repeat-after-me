using UnityEngine;
using Plamb.Events;

namespace Plamb.Character
{
	/// <summary>
	/// This component handles player animations.
	/// </summary>
    public class PlambComponentAnimation : MonoBehaviour
    {
	    // Define private members
	    private PlambComponentController m_plambController;
	    private Animator m_animator;
	    
	    private int m_animParamWalkSpeed;
	    private int m_animParamJump;
	    private int m_animParamLand;
	    private int m_animParamFall;
	    private int m_animParamReset;
	    
	    private bool m_fallAnimationBlocked;
	    
	    // Define serialized fields for animation parameter names and settings
	    [SerializeField] private string animationParameterNameWalkSpeed = "WalkSpeed";
	    [SerializeField] private string animationParameterNameJump = "Jump";
	    [SerializeField] private string animationParameterNameLand = "Land";
	    [SerializeField] private string animationParameterNameFall = "Fall";
	    [SerializeField] private string animationParameterNameReset = "Reset";
	    [SerializeField] private PlambSettings settings;
	    
        private void OnEnable()
        {
	        // Get references and animation values
	        m_plambController = GetComponentInParent<PlambComponentController>();
	        m_animator = GetComponent<Animator>();
	        
	        m_animParamWalkSpeed = Animator.StringToHash(animationParameterNameWalkSpeed);
	        m_animParamJump = Animator.StringToHash(animationParameterNameJump);
	        m_animParamLand = Animator.StringToHash(animationParameterNameLand);
	        m_animParamFall = Animator.StringToHash(animationParameterNameFall);
	        m_animParamReset = Animator.StringToHash(animationParameterNameReset);
	        
	        // Subscribe to events
	        EventBus.Subscribe<EventPlayerWalk>(OnPlayerWalk);
	        EventBus.Subscribe<EventPlayerJump>(OnPlayerJump);
	        EventBus.Subscribe<EventPlayerLand>(OnPlayerLand);
	        EventBus.Subscribe<EventPlayerFall>(OnPlayerFall);
	        EventBus.Subscribe<EventPlayerLoopReset>(OnPlayerLoopReset);
        }
        private void OnDisable()
        {
	        // Unsubscribe from events
	        EventBus.Unsubscribe<EventPlayerWalk>(OnPlayerWalk);
	        EventBus.Unsubscribe<EventPlayerJump>(OnPlayerJump);
	        EventBus.Unsubscribe<EventPlayerLand>(OnPlayerLand);
	        EventBus.Unsubscribe<EventPlayerFall>(OnPlayerFall);
	        EventBus.Unsubscribe<EventPlayerLoopReset>(OnPlayerLoopReset);
        }

        #region Handle animation events
        private void OnPlayerWalk(EventPlayerWalk e)
        {
	        if (e.CharacterID != m_plambController.CharacterID) return;
	        m_animator.SetFloat(m_animParamWalkSpeed, e.Speed);
        }
        private void OnPlayerJump(EventPlayerJump e)
        {
	        if (e.CharacterID != m_plambController.CharacterID) return;
	        m_animator.SetTrigger(m_animParamJump);
        }
        private void OnPlayerLand(EventPlayerLand e)
        {
	        if (e.CharacterID != m_plambController.CharacterID ||
	            e.VerticalVelocity > settings.landAnimationVelocityThreshold) return;
	        
	        m_animator.SetTrigger(m_animParamLand);
	        m_fallAnimationBlocked = false;
        }
        private void OnPlayerFall(EventPlayerFall e)
        {
	        if (e.CharacterID != m_plambController.CharacterID ||
	            e.VerticalVelocity > settings.fallAnimationVelocityThreshold ||
	            m_fallAnimationBlocked) return;
	        
	        m_animator.SetTrigger(m_animParamFall);
	        m_fallAnimationBlocked = true;
        }
        private void OnPlayerLoopReset(EventPlayerLoopReset e)
        {
	        if (e.CharacterID != m_plambController.CharacterID) return;
	        m_animator.SetFloat(m_animParamWalkSpeed, 0);
	        m_animator.SetTrigger(m_animParamReset);
        }
        #endregion
    }
}
