using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RollerAgent : Agent
{   
    public float speed = 10;
    public Transform target;


    public Rigidbody rBody;
    // Start is called before the first frame update
    void Start()
    {
      rBody = GetComponent<Rigidbody>();
    }

    // 进入新的一轮时调用的函数
    public override void OnEpisodeBegin()
    {   
        print("OnEpisodeBegin");
        // 当小球掉落的时候，重置小球的位置
        if (this.transform.localPosition.y < 0)
        {
            // 设置小球的初始位置
            this.transform.position = new Vector3(0, 0.5f, 0);
            // 设置小球的初始速度
            this.rBody.velocity = Vector3.zero;
            // 设置小球的初始角度
            this.rBody.angularVelocity = Vector3.zero;
        }

        // 设置目标的位置   
        target.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }

    // 收集观察的结果
    public override void CollectObservations(VectorSensor sensor)
    {   

        sensor.AddObservation(target.position);
        sensor.AddObservation(this.transform.position);

        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    // 接受动作，是否给予奖励
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {   
        // print("Horizontal"+actionBuffers.ContinuousActions.Array[0]);
        // print("Vertical"+actionBuffers.ContinuousActions.Array[1]);
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions.Array[0];
        controlSignal.z = actionBuffers.ContinuousActions.Array[1];
        rBody.AddForce(controlSignal * speed);

        // 出界
        if (this.transform.position.y < 0)
        {
            // print("Hit the ground");
            EndEpisode();
        }

        // 吃到东西了
        if (Vector3.Distance(this.transform.position, target.position) < 1.41f)
        {
            // print("Hit the target");
            // 给与奖励
            SetReward(1.0f);
            EndEpisode();
        }
    }

     public override void Heuristic(in ActionBuffers actionsOut)
    {
        actionsOut.ContinuousActions.Array[0]=Input.GetAxis("Horizontal");   // 获取水平轴的输入
        actionsOut.ContinuousActions.Array[1] = Input.GetAxis("Vertical");     // 获取垂直轴的输入
    }
}
