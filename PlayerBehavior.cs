using Unity.Netcode;
using UnityEngine;

public class PlayerBehavior : NetworkBehaviour
{
    private NetworkVariable<PlayerMovementData> _position = new (writePerm: NetworkVariableWritePermission.Owner);

    void Update()
    {
        if (IsOwner)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            transform.Translate(Time.deltaTime * vertical * transform.forward);
            transform.Translate(Time.deltaTime * horizontal * transform.right);

            _position.Value = new PlayerMovementData
            {
                Position = transform.position
            };
        }
        else
        {
            transform.position = _position.Value.Position;
        }
    }
}

internal struct PlayerMovementData : INetworkSerializable
{
    private float _x, _z;

    public Vector3 Position
    {
        get => new Vector3(_x, 0, _z);
        set
        {
            _x = value.x;
            _z = value.z;
        }
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _x);
        serializer.SerializeValue(ref _z);
    }
}