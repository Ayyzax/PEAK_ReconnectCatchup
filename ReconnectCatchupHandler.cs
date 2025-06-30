using System.Collections;
using BepInEx.Logging;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ayzax.ReconnectCatchup;

public struct ImprovedSpawnTarget
{
    public Character LowestCharacter;
    public readonly float LowestClimbed => LowestCharacter ? LowestCharacter.Center.y : float.PositiveInfinity;

    public void RegisterCharacter(Character character)
    {
        if (character.Center.y < LowestClimbed)
        {
            LowestCharacter = character;
        }
    }
}

public class ReconnectCatchupHandler : MonoBehaviourPunCallbacks
{
    ManualLogSource _logger;

    public void InitLogger(ManualLogSource logger)
    {
        _logger = logger;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (SceneManager.GetActiveScene().name == "Airport")
        {
            return;
        }

        StartCoroutine(DoImprovedReconnect(newPlayer));
    }

    IEnumerator DoImprovedReconnect(Photon.Realtime.Player newPlayer)
    {
        Character newCharacter = null;

        // Wait until character has been created
        while (newCharacter == null)
        {
            newCharacter = PlayerHandler.GetPlayerCharacter(newPlayer);
            yield return null;
        }

        ImprovedSpawnTarget spawnTarget = PopulateSpawnData(newCharacter);

        if (spawnTarget.LowestCharacter == null)
        {
            _logger.LogError("No valid spawn target found!");
            yield break;
        }

        _logger.LogInfo($"Moving {newCharacter.characterName} from [{newCharacter.Center}] to lowest character: {spawnTarget.LowestCharacter.characterName} [{spawnTarget.LowestCharacter.Center}]");

        newCharacter.photonView.RPC("WarpPlayerRPC", RpcTarget.All, spawnTarget.LowestCharacter.Head + Vector3.up, false);
    }

    static ImprovedSpawnTarget PopulateSpawnData(Character newCharacter)
    {
        ImprovedSpawnTarget spawnTarget = new ImprovedSpawnTarget();

        foreach (Character character in Character.AllCharacters)
        {
            if (!character.data.dead && character != newCharacter)
            {
                spawnTarget.RegisterCharacter(character);
            }
        }

        return spawnTarget;
    }
}