using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DentedPixel.LTExamples
{
	public class TestingUnitTests : MonoBehaviour
	{
		public GameObject cube1;

		public GameObject cube2;

		public GameObject cube3;

		public GameObject cube4;

		public GameObject cubeAlpha1;

		public GameObject cubeAlpha2;

		private bool eventGameObjectWasCalled;

		private bool eventGeneralWasCalled;

		private int lt1Id;

		private LTDescr lt2;

		private LTDescr lt3;

		private LTDescr lt4;

		private LTDescr[] groupTweens;

		private GameObject[] groupGOs;

		private int groupTweensCnt;

		private int rotateRepeat;

		private int rotateRepeatAngle;

		private GameObject boxNoCollider;

		private float timeElapsedNormalTimeScale;

		private float timeElapsedIgnoreTimeScale;

		private void Awake()
		{
			boxNoCollider = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(boxNoCollider.GetComponent(typeof(BoxCollider)));
		}

		private void Start()
		{
			LeanTest.timeout = 46f;
			LeanTest.expected = 56;
			LeanTween.init(1215);
			LeanTween.addListener(cube1, 0, eventGameObjectCalled);
			LeanTest.expect(!LeanTween.isTweening(), "NOTHING TWEEENING AT BEGINNING");
			LeanTest.expect(!LeanTween.isTweening(cube1), "OBJECT NOT TWEEENING AT BEGINNING");
			LeanTween.scaleX(cube4, 2f, 0f).setOnComplete((Action)delegate
			{
				LeanTest.expect(cube4.transform.localScale.x == 2f, "TWEENED WITH ZERO TIME");
			});
			LeanTween.dispatchEvent(0);
			LeanTest.expect(eventGameObjectWasCalled, "EVENT GAMEOBJECT RECEIVED");
			LeanTest.expect(!LeanTween.removeListener(cube2, 0, eventGameObjectCalled), "EVENT GAMEOBJECT NOT REMOVED");
			LeanTest.expect(LeanTween.removeListener(cube1, 0, eventGameObjectCalled), "EVENT GAMEOBJECT REMOVED");
			LeanTween.addListener(1, eventGeneralCalled);
			LeanTween.dispatchEvent(1);
			LeanTest.expect(eventGeneralWasCalled, "EVENT ALL RECEIVED");
			LeanTest.expect(LeanTween.removeListener(1, eventGeneralCalled), "EVENT ALL REMOVED");
			lt1Id = LeanTween.move(cube1, new Vector3(3f, 2f, 0.5f), 1.1f).id;
			LeanTween.move(cube2, new Vector3(-3f, -2f, -0.5f), 1.1f);
			LeanTween.reset();
			GameObject[] cubes = new GameObject[99];
			int[] tweenIds = new int[cubes.Length];
			for (int i = 0; i < cubes.Length; i++)
			{
				GameObject gameObject = cubeNamed("cancel" + i);
				tweenIds[i] = LeanTween.moveX(gameObject, 100f, 1f).id;
				cubes[i] = gameObject;
			}
			int onCompleteCount = 0;
			LeanTween.delayedCall(cubes[0], 0.2f, (Action)delegate
			{
				for (int j = 0; j < cubes.Length; j++)
				{
					if (j % 3 == 0)
					{
						LeanTween.cancel(cubes[j]);
					}
					else if (j % 3 == 1)
					{
						LeanTween.cancel(tweenIds[j]);
					}
					else if (j % 3 == 2)
					{
						LTDescr lTDescr2 = LeanTween.descr(tweenIds[j]);
						lTDescr2.setOnComplete((Action)delegate
						{
							onCompleteCount++;
							if (onCompleteCount >= 33)
							{
								LeanTest.expect(true, "CANCELS DO NOT EFFECT FINISHING");
							}
						});
					}
				}
			});
			Vector3[] pts = new Vector3[5]
			{
				new Vector3(-1f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(4f, 0f, 0f),
				new Vector3(20f, 0f, 0f),
				new Vector3(30f, 0f, 0f)
			};
			LTSpline lTSpline = new LTSpline(pts);
			lTSpline.place(cube4.transform, 0.5f);
			LeanTest.expect(Vector3.Distance(cube4.transform.position, new Vector3(10f, 0f, 0f)) <= 0.7f, "SPLINE POSITIONING AT HALFWAY", string.Concat("position is:", cube4.transform.position, " but should be:(10f,0f,0f)"));
			LeanTween.color(cube4, Color.green, 0.01f);
			GameObject gameObject2 = cubeNamed("cubeDest");
			Vector3 cubeDestEnd = new Vector3(100f, 20f, 0f);
			LeanTween.move(gameObject2, cubeDestEnd, 0.7f);
			GameObject cubeToTrans = cubeNamed("cubeToTrans");
			LeanTween.move(cubeToTrans, gameObject2.transform, 1.2f).setEase(LeanTweenType.easeOutQuad).setOnComplete((Action)delegate
			{
				LeanTest.expect(cubeToTrans.transform.position == cubeDestEnd, "MOVE TO TRANSFORM WORKS");
			});
			GameObject gameObject3 = cubeNamed("cubeDestroy");
			LeanTween.moveX(gameObject3, 200f, 0.05f).setDelay(0.02f).setDestroyOnComplete(true);
			LeanTween.moveX(gameObject3, 200f, 0.1f).setDestroyOnComplete(true).setOnComplete((Action)delegate
			{
				LeanTest.expect(true, "TWO DESTROY ON COMPLETE'S SUCCEED");
			});
			GameObject cubeSpline = cubeNamed("cubeSpline");
			LeanTween.moveSpline(cubeSpline, new Vector3[4]
			{
				new Vector3(0.5f, 0f, 0.5f),
				new Vector3(0.75f, 0f, 0.75f),
				new Vector3(1f, 0f, 1f),
				new Vector3(1f, 0f, 1f)
			}, 0.1f).setOnComplete((Action)delegate
			{
				LeanTest.expect(Vector3.Distance(new Vector3(1f, 0f, 1f), cubeSpline.transform.position) < 0.01f, "SPLINE WITH TWO POINTS SUCCEEDS");
			});
			GameObject jumpCube = cubeNamed("jumpTime");
			jumpCube.transform.position = new Vector3(100f, 0f, 0f);
			jumpCube.transform.localScale *= 100f;
			int jumpTimeId = LeanTween.moveX(jumpCube, 200f, 1f).id;
			LeanTween.delayedCall(base.gameObject, 0.2f, (Action)delegate
			{
				LTDescr lTDescr = LeanTween.descr(jumpTimeId);
				float beforeX = jumpCube.transform.position.x;
				lTDescr.setTime(0.5f);
				LeanTween.delayedCall(0f, (Action)delegate
				{
				}).setOnStart(delegate
				{
					float num = 1f;
					beforeX += Time.deltaTime * 100f * 2f;
					LeanTest.expect(Mathf.Abs(jumpCube.transform.position.x - beforeX) < num, "CHANGING TIME DOESN'T JUMP AHEAD", "Difference:" + Mathf.Abs(jumpCube.transform.position.x - beforeX) + " beforeX:" + beforeX + " now:" + jumpCube.transform.position.x + " dt:" + Time.deltaTime);
				});
			});
			GameObject zeroCube = cubeNamed("zeroCube");
			LeanTween.moveX(zeroCube, 10f, 0f).setOnComplete((Action)delegate
			{
				LeanTest.expect(zeroCube.transform.position.x == 10f, "ZERO TIME FINSHES CORRECTLY", "final x:" + zeroCube.transform.position.x);
			});
			GameObject cubeScale = cubeNamed("cubeScale");
			LeanTween.scale(cubeScale, new Vector3(5f, 5f, 5f), 0.01f).setOnStart(delegate
			{
				LeanTest.expect(true, "ON START WAS CALLED");
			}).setOnComplete((Action)delegate
			{
				LeanTest.expect(cubeScale.transform.localScale.z == 5f, "SCALE", "expected scale z:" + 5f + " returned:" + cubeScale.transform.localScale.z);
			});
			GameObject cubeRotate = cubeNamed("cubeRotate");
			LeanTween.rotate(cubeRotate, new Vector3(0f, 180f, 0f), 0.02f).setOnComplete((Action)delegate
			{
				LeanTest.expect(cubeRotate.transform.eulerAngles.y == 180f, "ROTATE", "expected rotate y:" + 180f + " returned:" + cubeRotate.transform.eulerAngles.y);
			});
			GameObject cubeRotateA = cubeNamed("cubeRotateA");
			LeanTween.rotateAround(cubeRotateA, Vector3.forward, 90f, 0.3f).setOnComplete((Action)delegate
			{
				LeanTest.expect(cubeRotateA.transform.eulerAngles.z == 90f, "ROTATE AROUND", "expected rotate z:" + 90f + " returned:" + cubeRotateA.transform.eulerAngles.z);
			});
			GameObject cubeRotateB = cubeNamed("cubeRotateB");
			cubeRotateB.transform.position = new Vector3(200f, 10f, 8f);
			LeanTween.rotateAround(cubeRotateB, Vector3.forward, 360f, 0.3f).setPoint(new Vector3(5f, 3f, 2f)).setOnComplete((Action)delegate
			{
				LeanTest.expect(cubeRotateB.transform.position.ToString() == new Vector3(200f, 10f, 8f).ToString(), "ROTATE AROUND 360", string.Concat("expected rotate pos:", new Vector3(200f, 10f, 8f), " returned:", cubeRotateB.transform.position));
			});
			LeanTween.alpha(cubeAlpha1, 0.5f, 0.1f).setOnUpdate(delegate(float val)
			{
				LeanTest.expect(val != 0f, "ON UPDATE VAL");
			}).setOnCompleteParam("Hi!")
				.setOnComplete(delegate(object completeObj)
				{
					LeanTest.expect((string)completeObj == "Hi!", "ONCOMPLETE OBJECT");
					LeanTest.expect(cubeAlpha1.GetComponent<Renderer>().material.color.a == 0.5f, "ALPHA");
				});
			float onStartTime = -1f;
			LeanTween.color(cubeAlpha2, Color.cyan, 0.3f).setOnComplete((Action)delegate
			{
				LeanTest.expect(cubeAlpha2.GetComponent<Renderer>().material.color == Color.cyan, "COLOR");
				LeanTest.expect(onStartTime >= 0f && onStartTime < Time.time, "ON START", "onStartTime:" + onStartTime + " time:" + Time.time);
			}).setOnStart(delegate
			{
				onStartTime = Time.time;
			});
			Vector3 beforePos = cubeAlpha1.transform.position;
			LeanTween.moveY(cubeAlpha1, 3f, 0.2f).setOnComplete((Action)delegate
			{
				LeanTest.expect(cubeAlpha1.transform.position.x == beforePos.x && cubeAlpha1.transform.position.z == beforePos.z, "MOVE Y");
			});
			Vector3 beforePos2 = cubeAlpha2.transform.localPosition;
			LeanTween.moveLocalZ(cubeAlpha2, 12f, 0.2f).setOnComplete((Action)delegate
			{
				LeanTest.expect(cubeAlpha2.transform.localPosition.x == beforePos2.x && cubeAlpha2.transform.localPosition.y == beforePos2.y, "MOVE LOCAL Z", "ax:" + cubeAlpha2.transform.localPosition.x + " bx:" + beforePos.x + " ay:" + cubeAlpha2.transform.localPosition.y + " by:" + beforePos2.y);
			});
			AudioClip audio = LeanAudio.createAudio(new AnimationCurve(new Keyframe(0f, 1f, 0f, -1f), new Keyframe(1f, 0f, -1f, 0f)), new AnimationCurve(new Keyframe(0f, 0.001f, 0f, 0f), new Keyframe(1f, 0.001f, 0f, 0f)), LeanAudio.options());
			LeanTween.delayedSound(base.gameObject, audio, new Vector3(0f, 0f, 0f), 0.1f).setDelay(0.2f).setOnComplete((Action)delegate
			{
				LeanTest.expect(Time.time > 0f, "DELAYED SOUND");
			});
			bool value2UpdateCalled = false;
			LeanTween.value(base.gameObject, new Vector2(0f, 0f), new Vector2(256f, 96f), 0.1f).setOnUpdate((Action<Vector2>)delegate
			{
				value2UpdateCalled = true;
			}, (object)null);
			LeanTween.delayedCall(0.2f, (Action)delegate
			{
				LeanTest.expect(value2UpdateCalled, "VALUE2 UPDATE");
			});
			StartCoroutine(timeBasedTesting());
		}

		private GameObject cubeNamed(string name)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(boxNoCollider);
			gameObject.name = name;
			return gameObject;
		}

		private IEnumerator timeBasedTesting()
		{
			yield return new WaitForEndOfFrame();
			GameObject cubeNormal = cubeNamed("normalTimeScale");
			LeanTween.moveX(cubeNormal, 12f, 1.5f).setIgnoreTimeScale(false).setOnComplete((Action)delegate
			{
				timeElapsedNormalTimeScale = Time.time;
			});
			LTDescr[] descr = LeanTween.descriptions(cubeNormal);
			LeanTest.expect(descr.Length >= 0 && descr[0].to.x == 12f, "WE CAN RETRIEVE A DESCRIPTION");
			GameObject cubeIgnore = cubeNamed("ignoreTimeScale");
			LeanTween.moveX(cubeIgnore, 5f, 1.5f).setIgnoreTimeScale(true).setOnComplete((Action)delegate
			{
				timeElapsedIgnoreTimeScale = Time.time;
			});
			yield return new WaitForSeconds(1.5f);
			LeanTest.expect(Mathf.Abs(timeElapsedNormalTimeScale - timeElapsedIgnoreTimeScale) < 0.7f, "START IGNORE TIMING", "timeElapsedIgnoreTimeScale:" + timeElapsedIgnoreTimeScale + " timeElapsedNormalTimeScale:" + timeElapsedNormalTimeScale);
			Time.timeScale = 4f;
			int pauseCount = 0;
			LeanTween.value(base.gameObject, 0f, 1f, 1f).setOnUpdate((Action<float>)delegate
			{
				pauseCount++;
			}).pause();
			Vector3[] roundCirc = new Vector3[16]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(-9.1f, 25.1f, 0f),
				new Vector3(-1.2f, 15.9f, 0f),
				new Vector3(-25f, 25f, 0f),
				new Vector3(-25f, 25f, 0f),
				new Vector3(-50.1f, 15.9f, 0f),
				new Vector3(-40.9f, 25.1f, 0f),
				new Vector3(-50f, 0f, 0f),
				new Vector3(-50f, 0f, 0f),
				new Vector3(-40.9f, -25.1f, 0f),
				new Vector3(-50.1f, -15.9f, 0f),
				new Vector3(-25f, -25f, 0f),
				new Vector3(-25f, -25f, 0f),
				new Vector3(0f, -15.9f, 0f),
				new Vector3(-9.1f, -25.1f, 0f),
				new Vector3(0f, 0f, 0f)
			};
			GameObject cubeRound = cubeNamed("bRound");
			Vector3 onStartPos = cubeRound.transform.position;
			LeanTween.moveLocal(cubeRound, roundCirc, 0.5f).setOnComplete((Action)delegate
			{
				LeanTest.expect(cubeRound.transform.position == onStartPos, "BEZIER CLOSED LOOP SHOULD END AT START", string.Concat("onStartPos:", onStartPos, " onEnd:", cubeRound.transform.position));
			});
			Vector3[] roundSpline = new Vector3[6]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(2f, 0f, 0f),
				new Vector3(0.9f, 2f, 0f),
				new Vector3(0f, 0f, 0f),
				new Vector3(0f, 0f, 0f)
			};
			GameObject cubeSpline = cubeNamed("bSpline");
			Vector3 onStartPosSpline = cubeSpline.transform.position;
			LeanTween.moveSplineLocal(cubeSpline, roundSpline, 0.5f).setOnComplete((Action)delegate
			{
				LeanTest.expect(Vector3.Distance(onStartPosSpline, cubeSpline.transform.position) <= 0.01f, "SPLINE CLOSED LOOP SHOULD END AT START", string.Concat("onStartPos:", onStartPosSpline, " onEnd:", cubeSpline.transform.position, " dist:", Vector3.Distance(onStartPosSpline, cubeSpline.transform.position)));
			});
			groupTweens = new LTDescr[1200];
			groupGOs = new GameObject[groupTweens.Length];
			groupTweensCnt = 0;
			int descriptionMatchCount = 0;
			for (int i = 0; i < groupTweens.Length; i++)
			{
				GameObject gameObject = cubeNamed("c" + i);
				gameObject.transform.position = new Vector3(0f, 0f, i * 3);
				groupGOs[i] = gameObject;
			}
			yield return new WaitForEndOfFrame();
			bool hasGroupTweensCheckStarted = false;
			int setOnStartNum = 0;
			int setPosNum = 0;
			bool setPosOnUpdate = true;
			for (int j = 0; j < groupTweens.Length; j++)
			{
				Vector3 vector = base.transform.position + Vector3.one * 3f;
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("final", vector);
				dictionary.Add("go", groupGOs[j]);
				Dictionary<string, object> onCompleteParam = dictionary;
				groupTweens[j] = LeanTween.move(groupGOs[j], vector, 3f).setOnStart(delegate
				{
					setOnStartNum++;
				}).setOnUpdate(delegate(Vector3 newPosition)
				{
					if (base.transform.position.z > newPosition.z)
					{
						setPosOnUpdate = false;
					}
				})
					.setOnCompleteParam(onCompleteParam)
					.setOnComplete(delegate(object param)
					{
						Dictionary<string, object> dictionary2 = param as Dictionary<string, object>;
						Vector3 vector2 = (Vector3)dictionary2["final"];
						GameObject gameObject3 = dictionary2["go"] as GameObject;
						if (vector2.ToString() == gameObject3.transform.position.ToString())
						{
							setPosNum++;
						}
						if (!hasGroupTweensCheckStarted)
						{
							hasGroupTweensCheckStarted = true;
							LeanTween.delayedCall(base.gameObject, 0.1f, (Action)delegate
							{
								LeanTest.expect(setOnStartNum == groupTweens.Length, "SETONSTART CALLS", "expected:" + groupTweens.Length + " was:" + setOnStartNum);
								LeanTest.expect(groupTweensCnt == groupTweens.Length, "GROUP FINISH", "expected " + groupTweens.Length + " tweens but got " + groupTweensCnt);
								LeanTest.expect(setPosNum == groupTweens.Length, "GROUP POSITION FINISH", "expected " + groupTweens.Length + " tweens but got " + setPosNum);
								LeanTest.expect(setPosOnUpdate, "GROUP POSITION ON UPDATE");
							});
						}
						groupTweensCnt++;
					});
				if (LeanTween.description(groupTweens[j].id).trans == groupTweens[j].trans)
				{
					descriptionMatchCount++;
				}
			}
			while (LeanTween.tweensRunning < groupTweens.Length)
			{
				yield return null;
			}
			LeanTest.expect(descriptionMatchCount == groupTweens.Length, "GROUP IDS MATCH");
			int expectedSearch = groupTweens.Length + 5;
			LeanTest.expect(LeanTween.maxSearch <= expectedSearch, "MAX SEARCH OPTIMIZED", "maxSearch:" + LeanTween.maxSearch + " should be:" + expectedSearch);
			LeanTest.expect(LeanTween.isTweening(), "SOMETHING IS TWEENING");
			float previousXlt4 = cube4.transform.position.x;
			lt4 = LeanTween.moveX(cube4, 5f, 1.1f).setOnComplete((Action)delegate
			{
				LeanTest.expect(cube4 != null && previousXlt4 != cube4.transform.position.x, "RESUME OUT OF ORDER", string.Concat("cube4:", cube4, " previousXlt4:", previousXlt4, " cube4.transform.position.x:", (!(cube4 != null)) ? 0f : cube4.transform.position.x));
			}).setDestroyOnComplete(true);
			lt4.resume();
			TestingUnitTests testingUnitTests = this;
			TestingUnitTests testingUnitTests2 = this;
			int num = 0;
			testingUnitTests2.rotateRepeatAngle = 0;
			testingUnitTests.rotateRepeat = num;
			LeanTween.rotateAround(cube3, Vector3.forward, 360f, 0.1f).setRepeat(3).setOnComplete(rotateRepeatFinished)
				.setOnCompleteOnRepeat(true)
				.setDestroyOnComplete(true);
			yield return new WaitForEndOfFrame();
			LeanTween.delayedCall(1.8f, rotateRepeatAllFinished);
			int countBeforeCancel = LeanTween.tweensRunning;
			LeanTween.cancel(lt1Id);
			LeanTest.expect(countBeforeCancel == LeanTween.tweensRunning, "CANCEL AFTER RESET SHOULD FAIL", "expected " + countBeforeCancel + " but got " + LeanTween.tweensRunning);
			LeanTween.cancel(cube2);
			int tweenCount = 0;
			for (int k = 0; k < groupTweens.Length; k++)
			{
				if (LeanTween.isTweening(groupGOs[k]))
				{
					tweenCount++;
				}
				if (k % 3 == 0)
				{
					LeanTween.pause(groupGOs[k]);
				}
				else if (k % 3 == 1)
				{
					groupTweens[k].pause();
				}
				else
				{
					LeanTween.pause(groupTweens[k].id);
				}
			}
			LeanTest.expect(tweenCount == groupTweens.Length, "GROUP ISTWEENING", "expected " + groupTweens.Length + " tweens but got " + tweenCount);
			yield return new WaitForEndOfFrame();
			tweenCount = 0;
			for (int l = 0; l < groupTweens.Length; l++)
			{
				if (l % 3 == 0)
				{
					LeanTween.resume(groupGOs[l]);
				}
				else if (l % 3 == 1)
				{
					groupTweens[l].resume();
				}
				else
				{
					LeanTween.resume(groupTweens[l].id);
				}
				if ((l % 2 != 0) ? LeanTween.isTweening(groupGOs[l]) : LeanTween.isTweening(groupTweens[l].id))
				{
					tweenCount++;
				}
			}
			LeanTest.expect(tweenCount == groupTweens.Length, "GROUP RESUME");
			LeanTest.expect(!LeanTween.isTweening(cube1), "CANCEL TWEEN LTDESCR");
			LeanTest.expect(!LeanTween.isTweening(cube2), "CANCEL TWEEN LEANTWEEN");
			LeanTest.expect(pauseCount == 0, "ON UPDATE NOT CALLED DURING PAUSE", "expect pause count of 0, but got " + pauseCount);
			yield return new WaitForEndOfFrame();
			Time.timeScale = 0.25f;
			float tweenTime = 0.2f;
			float expectedTime = tweenTime * (1f / Time.timeScale);
			float start = Time.realtimeSinceStartup;
			bool onUpdateWasCalled = false;
			LeanTween.moveX(cube1, -5f, tweenTime).setOnUpdate((Action<float>)delegate
			{
				onUpdateWasCalled = true;
			}).setOnComplete((Action)delegate
			{
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				float num2 = realtimeSinceStartup - start;
				LeanTest.expect(Mathf.Abs(expectedTime - num2) < 0.05f, "SCALED TIMING DIFFERENCE", "expected to complete in roughly " + expectedTime + " but completed in " + num2);
				LeanTest.expect(Mathf.Approximately(cube1.transform.position.x, -5f), "SCALED ENDING POSITION", "expected to end at -5f, but it ended at " + cube1.transform.position.x);
				LeanTest.expect(onUpdateWasCalled, "ON UPDATE FIRED");
			});
			bool didGetCorrectOnUpdate = false;
			LeanTween.value(base.gameObject, new Vector3(1f, 1f, 1f), new Vector3(10f, 10f, 10f), 1f).setOnUpdate(delegate(Vector3 val)
			{
				didGetCorrectOnUpdate = val.x >= 1f && val.y >= 1f && val.z >= 1f;
			}).setOnComplete((Action)delegate
			{
				LeanTest.expect(didGetCorrectOnUpdate, "VECTOR3 CALLBACK CALLED");
			});
			yield return new WaitForSeconds(expectedTime);
			Time.timeScale = 1f;
			int ltCount = 0;
			GameObject[] allGos = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
			GameObject[] array = allGos;
			foreach (GameObject gameObject2 in array)
			{
				if (gameObject2.name == "~LeanTween")
				{
					ltCount++;
				}
			}
			LeanTest.expect(ltCount == 1, "RESET CORRECTLY CLEANS UP");
			lotsOfCancels();
		}

		private IEnumerator lotsOfCancels()
		{
			yield return new WaitForEndOfFrame();
			Time.timeScale = 4f;
			int cubeCount = 10;
			int[] tweensA = new int[cubeCount];
			GameObject[] aGOs = new GameObject[cubeCount];
			for (int i = 0; i < aGOs.Length; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(boxNoCollider);
				gameObject.transform.position = new Vector3(0f, 0f, (float)i * 2f);
				gameObject.name = "a" + i;
				aGOs[i] = gameObject;
				tweensA[i] = LeanTween.move(gameObject, gameObject.transform.position + new Vector3(10f, 0f, 0f), 0.5f + 1f * (1f / (float)aGOs.Length)).id;
				LeanTween.color(gameObject, Color.red, 0.01f);
			}
			yield return new WaitForSeconds(1f);
			int[] tweensB = new int[cubeCount];
			GameObject[] bGOs = new GameObject[cubeCount];
			for (int j = 0; j < bGOs.Length; j++)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(boxNoCollider);
				gameObject2.transform.position = new Vector3(0f, 0f, (float)j * 2f);
				gameObject2.name = "b" + j;
				bGOs[j] = gameObject2;
				tweensB[j] = LeanTween.move(gameObject2, gameObject2.transform.position + new Vector3(10f, 0f, 0f), 2f).id;
			}
			for (int k = 0; k < aGOs.Length; k++)
			{
				LeanTween.cancel(aGOs[k]);
				GameObject gameObject3 = aGOs[k];
				tweensA[k] = LeanTween.move(gameObject3, new Vector3(0f, 0f, (float)k * 2f), 2f).id;
			}
			yield return new WaitForSeconds(0.5f);
			for (int l = 0; l < aGOs.Length; l++)
			{
				LeanTween.cancel(aGOs[l]);
				GameObject gameObject4 = aGOs[l];
				tweensA[l] = LeanTween.move(gameObject4, new Vector3(0f, 0f, (float)l * 2f) + new Vector3(10f, 0f, 0f), 2f).id;
			}
			for (int m = 0; m < bGOs.Length; m++)
			{
				LeanTween.cancel(bGOs[m]);
				GameObject gameObject5 = bGOs[m];
				tweensB[m] = LeanTween.move(gameObject5, new Vector3(0f, 0f, (float)m * 2f), 2f).id;
			}
			yield return new WaitForSeconds(2.1f);
			bool inFinalPlace = true;
			for (int n = 0; n < aGOs.Length; n++)
			{
				if (Vector3.Distance(aGOs[n].transform.position, new Vector3(0f, 0f, (float)n * 2f) + new Vector3(10f, 0f, 0f)) > 0.1f)
				{
					inFinalPlace = false;
				}
			}
			for (int num = 0; num < bGOs.Length; num++)
			{
				if (Vector3.Distance(bGOs[num].transform.position, new Vector3(0f, 0f, (float)num * 2f)) > 0.1f)
				{
					inFinalPlace = false;
				}
			}
			LeanTest.expect(inFinalPlace, "AFTER LOTS OF CANCELS");
		}

		private void rotateRepeatFinished()
		{
			if (Mathf.Abs(cube3.transform.eulerAngles.z) < 0.0001f)
			{
				rotateRepeatAngle++;
			}
			rotateRepeat++;
		}

		private void rotateRepeatAllFinished()
		{
			LeanTest.expect(rotateRepeatAngle == 3, "ROTATE AROUND MULTIPLE", "expected 3 times received " + rotateRepeatAngle + " times");
			LeanTest.expect(rotateRepeat == 3, "ROTATE REPEAT", "expected 3 times received " + rotateRepeat + " times");
			LeanTest.expect(cube3 == null, "DESTROY ON COMPLETE", "cube3:" + cube3);
		}

		private void eventGameObjectCalled(LTEvent e)
		{
			eventGameObjectWasCalled = true;
		}

		private void eventGeneralCalled(LTEvent e)
		{
			eventGeneralWasCalled = true;
		}
	}
}
