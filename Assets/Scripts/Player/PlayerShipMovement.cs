using System;
using UnityEngine;
using Unity.Netcode;

public class PlayerShipMovement : NetworkBehaviour{
    private enum MoveType{
        Constant,
        Momentum
    }

    private enum VerticalMovementType{
        None,
        Upward,
        Downward
    }

    [Serializable]
    public struct PlayerLimits{
        public float minLimit;
        public float maxLimit;
    }

    [SerializeField] private MoveType m_moveType = MoveType.Momentum;
    [SerializeField] private PlayerLimits m_verticalLimits;
    [SerializeField] private PlayerLimits m_hortizontalLimits;

    [Header("ShipSprites")]
    [SerializeField] private SpriteRenderer m_shipRenderer;
    [SerializeField] private Sprite m_normalSprite;
    [SerializeField] private Sprite m_upSprite;
    [SerializeField] private Sprite m_downSprite;
    [SerializeField] private float m_speed;

    private float m_inputX;
    private float m_inputY;

    private VerticalMovementType m_previousVerticalMovementType = VerticalMovementType.None;
    private VerticalMovementType m_currentVerticalMovementType = VerticalMovementType.None;

    private const string k_horizontalAxis = "Horizontal";
    private const string k_verticalAxis = "Vertical";

    // Update is called once per frame
    private void Update(){
        // We're only updating the ship's movements when we're surely updating on the owning
        // instance
        if (!IsOwner)
            return;

        HandleKeyboardInput();

        UpdateVerticalMovementSprite();

        AdjustInputValuesBasedOnPositionLimits();

        MovePlayerShip();
    }

    private void HandleKeyboardInput(){
        switch (m_moveType){
            /*
            There two types of movement:
            constant -> linear move, there are no time acceleration
            momentum -> move with acceleration at the start

            Note: feel free to add your own type as well
        */
            case MoveType.Constant:
                HandleMoveTypeConstant();
                break;

            case MoveType.Momentum:
                HandleMoveTypeMomentum();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void HandleMoveTypeConstant(){
        m_inputX = 0f;
        m_inputY = 0f;

        // Horizontal input
        if (Input.GetKey(KeyCode.D)){
            m_inputX = 1f;
        }
        else if (Input.GetKey(KeyCode.A)){
            m_inputX = -1f;
        }

        // Vertical input and set the ship sprite
        if (Input.GetKey(KeyCode.W)){
            m_inputY = 1f;
        }
        else if (Input.GetKey(KeyCode.S)){
            m_inputY = -1f;
        }
    }

    private void HandleMoveTypeMomentum(){
        m_inputX = Input.GetAxis(k_horizontalAxis);
        m_inputY = Input.GetAxis(k_verticalAxis);
    }

    private void UpdateVerticalMovementSprite(){
        m_previousVerticalMovementType = m_currentVerticalMovementType;

        UpdateCurrentVerticalMovementType();

        if (m_currentVerticalMovementType != m_previousVerticalMovementType){
            // inform the server of the update to vertical movement type
            NewVerticalMovementServerRPC(m_currentVerticalMovementType);
        }
    }

    private void UpdateCurrentVerticalMovementType(){
        switch (m_inputY){
            // Change the ship sprites base on the input value
            case > 0f:
                m_shipRenderer.sprite = m_upSprite;
                m_currentVerticalMovementType = VerticalMovementType.Upward;
                break;

            case < 0f:
                m_shipRenderer.sprite = m_downSprite;
                m_currentVerticalMovementType = VerticalMovementType.Downward;
                break;

            default:
                m_shipRenderer.sprite = m_normalSprite;
                m_currentVerticalMovementType = VerticalMovementType.None;
                break;
        }
    }

    [ServerRpc]
    private void NewVerticalMovementServerRPC(VerticalMovementType newVerticalMovementType){
        // The server lets all other clients of this ship's new vertical movement
        NewVerticalMovementClientRPC(newVerticalMovementType);
    }

    [ClientRpc]
    private void NewVerticalMovementClientRPC(VerticalMovementType newVerticalMovementType){
        m_shipRenderer.sprite = newVerticalMovementType switch{
            VerticalMovementType.None => m_normalSprite,
            VerticalMovementType.Upward => m_upSprite,
            VerticalMovementType.Downward => m_downSprite,
            _ => m_shipRenderer.sprite
        };
    }

    // Check the limits of the player and adjust the input
    private void AdjustInputValuesBasedOnPositionLimits(){
        PlayerMovementInputLimitAdjuster.AdjustInputValuesBasedOnPositionLimits(
            transform.position,
            ref m_inputX,
            ref m_inputY,
            m_hortizontalLimits,
            m_verticalLimits
        );
    }

    private void MovePlayerShip(){
        // Take the value from the input and multiply by speed and time
        var speedTimesDeltaTime = m_speed * Time.deltaTime;

        var newYposition = m_inputY * speedTimesDeltaTime;
        var newXposition = m_inputX * speedTimesDeltaTime;

        // move the ship
        transform.Translate(newXposition, newYposition, 0f);
    }
}