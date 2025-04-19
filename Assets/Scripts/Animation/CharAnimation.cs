using UnityEngine;

public class CharAnimation : MonoBehaviour
{
    private Character character;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character = GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        ChoseAnimation(character);
    }

    private void ChoseAnimation(Character c)
    {
        c.Anim.SetBool("IsIdle", false);
        c.Anim.SetBool("IsWalk", false);

        switch (c.State)
        {
            case CharState.Idle:
                c.Anim.SetBool("IsIdle", true);
                break;
            case CharState.Walk:
                c.Anim.SetBool("IsWalk", true);
                break;
            case CharState.WalkToEnemy:
                c.Anim.SetBool("IsWalk", true);
                break;
            case CharState.WalkToMagicCast:
                c.Anim.SetBool("IsWalk", true);
                break;
            case CharState.WalkToNPC:
                c.Anim.SetBool("IsWalk", true);
                break;
        }
    }
}
