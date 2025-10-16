using UnityEngine;

public class JumpExitLock : StateMachineBehaviour
{
    [Tooltip("Nombre del parámetro bool que habilita salir de Jump")]
    public string canExitParam = "CanExitJump";

    [Tooltip("Cuánto del clip debe reproducirse para permitir la salida (1 = todo)")]
    [Range(0.0f, 1.0f)] public float normalizedEnd = 1.0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Bloquea salida al entrar en Jump
        if (!string.IsNullOrEmpty(canExitParam))
            animator.SetBool(canExitParam, false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Cuando se haya reproducido el % indicado del clip → permitir salida
        if (!string.IsNullOrEmpty(canExitParam) && stateInfo.normalizedTime >= normalizedEnd)
            animator.SetBool(canExitParam, true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // (opcional) dejarlo en false para el siguiente salto
        if (!string.IsNullOrEmpty(canExitParam))
            animator.SetBool(canExitParam, false);
    }
}
