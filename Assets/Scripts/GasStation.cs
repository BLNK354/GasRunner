using UnityEngine;

public class GasStation : MonoBehaviour
{
	private bool activated = false;

	private void OnTriggerEnter(Collider other)
	{
		if (activated) return;

		if (other.CompareTag("Player"))
		{
			activated = true;
			GameManager.Instance.ReachStation();
			Debug.Log("Gas station reached!");
		}
	}
}