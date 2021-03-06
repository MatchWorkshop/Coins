using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UniRx;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private Image frame;
    [SerializeField] private Text captain;
    [SerializeField] private Text content;
    [SerializeField] private Text cost;
    public LinkedListNode<CardUI> node { get; private set; }
    public Guid guid { get; private set; }
    IDisposable upDateTransfrom;
    private Vector3 positionShouldBe;
    private Quaternion quaternionShouldBe;
    private int siblingIndex;
    private float speed=4f;
    private bool MoveDone
    {
        get
        {
            var IsArriveRightPosition = positionShouldBe == transform.localPosition;
            var IsArriveRightAngle = quaternionShouldBe == transform.localRotation;
            return IsArriveRightAngle && IsArriveRightPosition;
        }
    }
    private bool player;
    private Action<Guid> useCard;
    public void Initialize(LinkedListNode<CardUI> linkedListNode,bool player)
    {
        if (player)
        {
            useCard = BattleSystem.PlayerUseCard;
        }
        else
        {
            useCard = BattleSystem.EnemyUseCard;
        }
        this.player = player;
        node = linkedListNode;
    }
    public void SetCard(Guid guid, Card card,CardUIInfo info)
    {
        this.guid = guid;
        bg.sprite = info.bg;
        frame.sprite = info.card;
        captain.text = card.type.ToString();
        content.text = card.GetContent();
        cost.text = card.cost.ToString();
    }
    public void SetCardPosition(Vector3 position,Quaternion quaternion)
    {
        positionShouldBe = position;
        quaternionShouldBe = quaternion;
        EveryUpdateTransform();
    }

    private void EveryUpdateTransform()
    {
        upDateTransfrom?.Dispose();
        upDateTransfrom = Observable.EveryUpdate()
            .Subscribe(_ => LerpCardTransform());
    }

    private void LerpCardTransform()
    {
        float moveSpeen = Time.deltaTime* speed;
        var tempAngle = Quaternion.Lerp(transform.localRotation, quaternionShouldBe, moveSpeen);
        var tempPosition = Vector3.Lerp(transform.localPosition, positionShouldBe, moveSpeen);
        transform.localPosition = tempPosition;
        transform.localRotation = tempAngle;
        
        if (MoveDone)
        {
            upDateTransfrom.Dispose();
        }
    }


    public void OnMouseEnter()
    {
        siblingIndex=transform.GetSiblingIndex();
        transform.SetSiblingIndex(-1);
    }
    public void OnMouseExit()
    {
        transform.SetSiblingIndex(siblingIndex);
    }
    public void OnMouseDrag()
    {
        transform.position = Input.mousePosition;
    }

    public void OnMouseUp()
    {
        if (BattleSystem.PlayerTurn== player && Vector3.Distance(transform.position, positionShouldBe) > 1000)
        {
            useCard?.Invoke(guid);
        }
        else
        {
            EveryUpdateTransform();
        }
    }
}