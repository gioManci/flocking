using UnityEngine;
using System.Collections;
namespace Flock{
	class Boid : MonoBehaviour{
		public FlockGlobals FLOCKGLOB;
		Vector2 velocity;
		Vector2? target;
		public static float speed = 3f;
		public static float rotationSpeed = 10f;

		public void Start() {
			FLOCKGLOB = FlockGlobals.instance;
			target = null;
			StartCoroutine( recalculateDirection());
		} 
		public void Update() {
			if(Input.GetKeyDown(KeyCode.Mouse0)){
				if(Random.Range(0f,1f)<0.5f){
					GetComponent<SpriteRenderer>().material.color = Color.red;
					target = Camera.main.ScreenToWorldPoint( Input.mousePosition );
				}
				else{
					GetComponent<SpriteRenderer>().material.color = Color.white;
					target = null;
				}
			}

			//float angle = Vector3.Angle( transform.up, dir);
			//if( Vector2.Dot(transform.up,dir)<0f ) angle = 180f-angle;
			//Debug.Log( "Desired direction for flockling is "+dir );
			//transform.Rotate( Vector3.Cross(transform.up, dir) ,angle*rotationSpeed*3f*Time.deltaTime );
			//transform.Translate( transform.up*speed*Time.deltaTime);
			transform.position += (Vector3)dir*speed*Time.deltaTime;
			transform.up = Vector2.Lerp(dir,transform.up, 0.5f*Time.deltaTime);

		}
		Vector2 dir;

		IEnumerator recalculateDirection(){
			while(true){
				//Collider2D[] flocklings = Physics2D.OverlapCircleAll(transform.position, FLOCKGLOB.SIGHT_RADIUS, 
				//LayerMask.NameToLayer("Flocklings"));
				Collider2D[] flocklings = Physics2D.OverlapCircleAll( transform.position, FLOCKGLOB.SIGHT_RADIUS);
				Vector2 position = (Vector2)transform.position, tmp;
				Vector2 align=Vector2.zero, cohesion=position, separation=Vector2.zero;
				float scale = (float)flocklings.Length -1f;
				int count = 0;
				foreach (Collider2D c in flocklings ) {
					tmp = position - (Vector2) c.transform.position;
					if(c.gameObject.tag != "Flockling" 
							|| Vector2.Dot(transform.up,-(tmp.normalized))<FLOCKGLOB.VISION_ANGLE 
							) 
						continue;
					align +=(Vector2) c.transform.up;
					if (position!=(Vector2)c.transform.position){
						separation += tmp.normalized/tmp.magnitude;
					}
					cohesion += (Vector2)c.transform.position;
					++count;
				}
				Vector2 center;
				scale = Mathf.Clamp( scale, 1f, float.PositiveInfinity);
				scale = (float)count;
				center = (Vector2)cohesion/scale;
				align/=scale;
				//Debug.Log("Count "+count);
				//Debug.Log("Align: " + align);
				//Debug.Log("Separation " +separation);
				//Debug.Log("Center " + center);
				//Debug.Log("Cohesion " + (center - position));
				dir = FLOCKGLOB.ALIGNMENT*align
					+ FLOCKGLOB.SEPARATION*separation
					+ FLOCKGLOB.COHESION*(center - position);

				dir.Normalize();
				if(target!=null){
				dir += (target.Value - center).normalized;
				} else {
					dir+= (center-position).normalized;
				}
				dir.Normalize();
				//Debug.DrawLine( transform.position, transform.position+ (Vector3) dir*4f);
				yield return null;}
		}
	}}
