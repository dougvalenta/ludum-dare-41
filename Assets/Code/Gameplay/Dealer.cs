using System.Collections;
using UnityEngine;

public class Dealer : MonoBehaviour {

    static readonly Quaternion faceDown = Quaternion.Euler(0f, 0f, 180f);
    static readonly Quaternion upsideDown = Quaternion.Euler(0f, 180f, 0f);
    static readonly Quaternion faceDownUpsideDown = Quaternion.Euler(0f, 180f, 180f);

    Card[] cards;

    public float dealSpeed = 5f;
    public Transform[] stackPositions;
    public Transform deckPosition;
    public float cardThickness = 0.002f;
    public float cardOffset = 0.01f;
    public float jitter = 0.001f;

    public Card prefab;
    public Mesh[] pipsMeshes;
    public Material[] suitMaterials;
    public Mesh[] faceMeshes;
    public Material faceMaterial;

    Transform _transform;

    bool dealing;

	private void Start()
	{
        _transform = transform;
        Deal();
	}

    public void Deal() {
        if (dealing) return;
        dealing = true;
        if (cards == null) {
            cards = new Card[52];
            for (int i = 0; i < 52; i++) {
                cards[i] = CreateCard(i);
            }
        }
        for (int i = 0; i < 52; i++) {
            Card card = cards[i];
            int j = Random.Range(i, 52);
            cards[i] = cards[j];
            cards[j] = card;
            Rigidbody cardBody = cards[i].GetComponent<Rigidbody>();
            cardBody.isKinematic = true;
            cards[i].transform.position = _transform.position;
            cards[i].enabled = false;
        }
        StartCoroutine(DoDeal());
    }

    IEnumerator DoDeal() {
        FlashMessage.main.FadeOut();
        int cardToDeal = 0;
        for (int level = 0; level < stackPositions.Length; level++)
        {
            for (int stack = level; stack < stackPositions.Length; stack++)
            {
                Vector3 stackPosition = stackPositions[stack].position;
                Vector3 position = new Vector3(stackPosition.x, stackPosition.y + cardThickness * level, stackPosition.z - cardOffset * level);
                Quaternion rotation;
                if (stack == level) {
                    rotation = Random.Range(0, 2) == 0 ? Quaternion.identity : upsideDown;
                } else {
                    rotation = Random.Range(0, 2) == 0 ? faceDown : faceDownUpsideDown;
                }
                yield return DealCard(cardToDeal, position, rotation, dealSpeed);
                cardToDeal++;
            }    
        }
        Vector3 deckCardPosition = deckPosition.position;
        while (cardToDeal < 52) {
            yield return DealCard(cardToDeal, deckCardPosition, Random.Range(0, 2) == 0 ? faceDown : faceDownUpsideDown, dealSpeed * 2f);
            deckCardPosition.y += cardThickness;
            cardToDeal++;
        }
        if (!Card.hasEverClicked) {
            FlashMessage.main.FadeIn("Click and drag to pick up and move a card.", 0f);
        }
        for (int i = 0; i < 52; i++) {
            cards[i].enabled = true;
        }
        dealing = false;
    }

    IEnumerator DealCard(int index, Vector3 position, Quaternion rotation, float speed) {
        yield return new WaitForFixedUpdate();
        Vector2 cardJitter = Random.insideUnitCircle * jitter;
        position.x += cardJitter.x;
        position.z += cardJitter.y;
        Card card = cards[index];
        float t = Time.fixedDeltaTime * speed;
        Rigidbody cardBody = card.GetComponent<Rigidbody>();
        cardBody.MoveRotation(rotation);
        Vector3 cardPosition = _transform.position;
        while (t < 1f) {
            cardBody.MovePosition(Vector3.Lerp(cardPosition, position, t));
            yield return new WaitForFixedUpdate();
            t += Time.fixedDeltaTime * speed;
        }
        cardBody.MovePosition(position);
        yield return new WaitForFixedUpdate();
        cardBody.velocity = Vector3.zero;
        cardBody.isKinematic = false;
    }

	Card CreateCard(int index) {
        int rank = index % 13;
        int suit = index / 13;
        Card card = Instantiate(prefab, _transform.position, Quaternion.identity);
        GameObject pips = new GameObject("Pips", typeof(MeshFilter), typeof(MeshRenderer));
        pips.GetComponent<MeshFilter>().sharedMesh = pipsMeshes[rank];
        MeshRenderer pipsRenderer = pips.GetComponent<MeshRenderer>();
        pipsRenderer.sharedMaterial = suitMaterials[suit];
        pipsRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        pips.transform.parent = card.transform;
        pips.transform.localPosition = Vector3.zero;
        pips.layer = 8;
        if (rank > 9) {
            GameObject face = new GameObject("Face", typeof(MeshFilter), typeof(MeshRenderer));
            face.GetComponent<MeshFilter>().sharedMesh = faceMeshes[rank - 10];
            MeshRenderer faceRenderer = face.GetComponent<MeshRenderer>();
            faceRenderer.sharedMaterial = faceMaterial;
            faceRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            face.transform.parent = card.transform;
            face.transform.localPosition = Vector3.zero;
            face.layer = 8;
        }
        return card;
    }

}
