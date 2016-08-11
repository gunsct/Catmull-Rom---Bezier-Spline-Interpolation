using UnityEngine;
using System.Collections;

public class SlpineMove : MonoBehaviour {
	//스팟과 라인 그릴 오브젝트
	public int point = 11;
	public GameObject[] pos;
	public GameObject line;

	//0~1(최초점~최종점)간격과 좌표 담을 변수들
	float u = 0.0f;
	float[] xpos,ypos,zpos;
	Vector3[] vec3;

	// Use this for initialization
	void Start () {
		xpos = new float[point + 1];
		ypos = new float[point + 1];
		zpos = new float[point + 1];
		vec3 = new Vector3[point + 1];

		for (int i = 0; i < point; i++) {
			xpos [i] = pos [i].transform.position.x;
			ypos [i] = pos [i].transform.position.y;
			zpos [i] = pos [i].transform.position.z;
			vec3 [i] = pos [i].transform.position;
		}

		StartCoroutine ("Move");
	}

	IEnumerator Move(){
		yield return new WaitForSeconds (0.05f);
		u += 0.01f;

		//3차원에서 2차원만 쓰고싶으면 vector2를 쓰던가 그냥 아래에서 x,y만 쓰던가 하면 됨

		//Catmull Rom 보간
		//xpos [4] = 0.5f * ((2.0f * xpos [1]) + (-xpos [0] + xpos [2]) * u + (2.0f * xpos [0] - 5.0f * xpos [1] + 4.0f * xpos [2] - xpos [3]) * u * u + (-xpos [0] + 3.0f * xpos [1] - 3.0f * xpos [2] + xpos [3]) * u * u * u);
		//ypos[4] = 0.5f * ((2.0f * ypos [1]) + (-ypos [0] + ypos [2]) * u + (2.0f * ypos [0] - 5.0f * ypos [1] + 4.0f * ypos [2] - ypos [3]) * u * u + (-ypos [0] + 3.0f * ypos [1] - 3.0f * ypos [2] + ypos [3]) * u * u * u);

		//베지어 스플라인 보간
		//둘다 점4개를 쓸 수 있지만 더 세밀한 컨트롤은 베지어 스플라인이 아닐까 판단됨
		BSpline(point,u);

		//this.transform.position = new Vector3 (xpos[point], ypos[point], zpos[point]);
		this.transform.position = vec3 [point];
		Instantiate (line, this.transform.position, Quaternion.identity);

		if(u <= 1.0f) StartCoroutine ("Move");
	}

	//http://3dmpengines.tistory.com/784 베지어스플라인, http://wiki.mathnt.net/index.php?title=%EC%9D%B4%ED%95%AD%EA%B3%84%EC%88%98%EC%99%80_%EC%A1%B0%ED%95%A9 이항계수 유도공식
	void BSpline(int _pnum, float _u){//스팟간의 간격들을 보간해 좌표 계산해줌 스팟 수와 그에 따른 이항계수 계산 후 위치정도(u)를 이용해 좌표를 도출
		//xpos[_pnum] = 0.0f;
		//ypos[_pnum] = 0.0f;
		vec3 [_pnum] = new Vector3 (0.0f, 0.0f, 0.0f);

		for (int i = 0; i < _pnum; i++) {
			//xpos [_pnum] += UF(_pnum,i) * xpos [i] * Mathf.Pow (1 - _u, _pnum - 1 - i) * Mathf.Pow (_u, i);
			//ypos [_pnum] += UF(_pnum,i) * ypos [i] * Mathf.Pow (1 - _u, _pnum - 1 - i) * Mathf.Pow (_u, i);
			//zpos [_pnum] += UF(_pnum,i) * zpos [i] * Mathf.Pow (1 - _u, _pnum - 1 - i) * Mathf.Pow (_u, i);
			vec3 [_pnum] += UF(_pnum,i) * vec3 [i] * Mathf.Pow (1 - _u, _pnum - 1 - i) * Mathf.Pow (_u, i);
		}
	}

	float UF(int _pnum, int _num){//이항계수 유도 공식
		if (_num.Equals (0) || _num.Equals (_pnum - 1)) return 1.0f;
		
		float num = Factorial (_pnum - 1) / (Factorial (_num) * Factorial (_pnum - 1 - _num));

		return num;
	}

	int Factorial(int _num){//팩토리얼 공식
		int num = 1;

		for (int i = _num; i > 1; i--) {
			num *= i;
		}

		return num;
	}
}
