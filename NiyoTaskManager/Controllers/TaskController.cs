﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NiyoTaskManager.Core.Implementations;
using NiyoTaskManager.Core.Interfaces;
using NiyoTaskManager.Core.Utilities;
using NiyoTaskManager.Data.DTOs.Auth;
using NiyoTaskManager.Data.DTOs.TaskManagement;
using NiyoTaskManager.Data.Entities;

namespace NiyoTaskManager.API.Controllers
{
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        [AllowAnonymous]
        [HttpPost("task/create")]
        [ProducesResponseType(200, Type = typeof(SignInResultDTO))]
        [ProducesResponseType(400, Type = typeof(List<string>))]
        public async Task<IActionResult> CreatNewTask([FromBody] NewTaskDTO model)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var response = await _taskService.CreateTaskAsync(model);
                    return Ok(new NiyoAPICustomResponseSchema("Task Creation Successful", response));
                }
                else
                {
                    return BadRequest(new NiyoAPICustomResponseSchema(new List<string>()));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new NiyoAPICustomResponseSchema(new List<string>() { ex.Message }));
            }
        }
        [AllowAnonymous]
        [HttpGet("task/fetch/{id}")]
        [ProducesResponseType(200, Type = typeof(SignInResultDTO))]
        [ProducesResponseType(400, Type = typeof(List<string>))]
        public async Task<IActionResult> FetchTask([FromRoute] string id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _taskService.FetchTaskAsync(id);
                    return Ok(new NiyoAPICustomResponseSchema("Task Retrieval Successful", response));
                }
                else
                {
                    return BadRequest(new NiyoAPICustomResponseSchema(new List<string>()));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new NiyoAPICustomResponseSchema(new List<string>() { ex.Message }));
            }
        }
        [AllowAnonymous]
        [HttpGet("task/fetchall")]
        [ProducesResponseType(200, Type = typeof(SignInResultDTO))]
        [ProducesResponseType(400, Type = typeof(List<string>))]
        public async Task<IActionResult> FetchAllTasks()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _taskService.FetchAllTasksAsync();
                    return Ok(new NiyoAPICustomResponseSchema("Tasks Retrieval Successful", response));
                }
                else
                {
                    return BadRequest(new NiyoAPICustomResponseSchema(new List<string>()));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new NiyoAPICustomResponseSchema(new List<string>() { ex.Message }));
            }
        }
        [AllowAnonymous]
        [HttpPost("task/delete/{id}")]
        [ProducesResponseType(200, Type = typeof(SignInResultDTO))]
        [ProducesResponseType(400, Type = typeof(List<string>))]
        public async Task<IActionResult> DeleteTask([FromRoute] string id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _taskService.DeleteTaskAsync(id);
                    return Ok(new NiyoAPICustomResponseSchema("Task deletion Successful"));
                }
                else
                {
                    return BadRequest(new NiyoAPICustomResponseSchema(new List<string>()));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new NiyoAPICustomResponseSchema(new List<string>() { ex.Message }));
            }
        }
        [AllowAnonymous]
        [HttpPost("task/update")]
        [ProducesResponseType(200, Type = typeof(SignInResultDTO))]
        [ProducesResponseType(400, Type = typeof(List<string>))]
        public async Task<IActionResult> UpdateTask([FromBody] TaskUpdateDTO model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _taskService.UpdateTaskAsync(model);
                    return Ok(new NiyoAPICustomResponseSchema("Task Update Successful", response));
                }
                else
                {
                    return BadRequest(new NiyoAPICustomResponseSchema(new List<string>()));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new NiyoAPICustomResponseSchema(new List<string>() { ex.Message }));
            }
        }
    }
}
