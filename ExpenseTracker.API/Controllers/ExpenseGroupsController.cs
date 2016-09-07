using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Routing;
using ExpenseTracker.API.Helpers;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Entities;
using ExpenseTracker.Repository.Factories;
using Marvin.JsonPatch;
using Newtonsoft.Json;
using ExpenseGroup = ExpenseTracker.DTO.ExpenseGroup;

namespace ExpenseTracker.API.Controllers
{
    // This enables Cross-Origin Resource Sharing
    [EnableCors("*", "*", "GET,POST")]
    public class ExpenseGroupsController : ApiController
    {
        private const int MaxPageSize = 10;
        private readonly ExpenseGroupFactory _expenseGroupFactory = new ExpenseGroupFactory();
        private readonly IExpenseTrackerRepository _repository;

        public ExpenseGroupsController()
        {
            _repository = new ExpenseTrackerEfRepository(new ExpenseTrackerContext());
        }

        public ExpenseGroupsController(IExpenseTrackerRepository repository)
        {
            _repository = repository;
        }


        [Route("api/expensegroups", Name = "ExpenseGroupsList")]
        public IHttpActionResult Get(string sort = "id", string status = null, string userId = null,
            string fields = null, int page = 1, int pageSize = MaxPageSize)
        {
            try
            {
                bool includeExpenses = false;
                List<string> lstOfFields = new List<string>();

                // we should include expenses when the fields-string contains "expenses", or "expenses.id", …
                if (fields != null)
                {
                    lstOfFields = fields.ToLower().Split(',').ToList();
                    includeExpenses = lstOfFields.Any(f => f.Contains("expenses"));
                }

                int statusId = -1;
                if (status != null)
                {
                    switch (status.ToLower())
                    {
                        case "open":
                            statusId = 1;
                            break;
                        case "confirmed":
                            statusId = 2;
                            break;
                        case "processed":
                            statusId = 3;
                            break;
                    }
                }

                // get expensegroups from repository
                IQueryable<Repository.Entities.ExpenseGroup> expenseGroups = null;
                expenseGroups = includeExpenses ? _repository.GetExpenseGroupsWithExpenses() : _repository.GetExpenseGroups();


                expenseGroups = expenseGroups
                    .ApplySort(sort)
                    .Where(eg => statusId == -1 || eg.ExpenseGroupStatusId == statusId)
                    .Where(eg => userId == null || eg.UserId == userId);


                // ensure the page size isn't larger than the maximum.
                if (pageSize > MaxPageSize)
                {
                    pageSize = MaxPageSize;
                }

                // calculate data for metadata
                int totalCount = expenseGroups.Count();
                var totalPages = (int) Math.Ceiling((double) totalCount/pageSize);

                var urlHelper = new UrlHelper(Request);
                string prevLink = page > 1
                    ? urlHelper.Link("ExpenseGroupsList",
                        new
                        {
                            page = page - 1,
                            pageSize,
                            sort,
                            fields,
                            status,
                            userId
                        })
                    : "";
                string nextLink = page < totalPages
                    ? urlHelper.Link("ExpenseGroupsList",
                        new
                        {
                            page = page + 1,
                            pageSize,
                            sort,
                            fields,
                            status,
                            userId
                        })
                    : "";


                var paginationHeader = new
                {
                    currentPage = page,
                    pageSize,
                    totalCount,
                    totalPages,
                    previousPageLink = prevLink,
                    nextPageLink = nextLink
                };

                HttpContext.Current.Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationHeader));


                // return result
                return Ok(expenseGroups
                    .Skip(pageSize*(page - 1))
                    .Take(pageSize)
                    .ToList()
                    .Select(eg => _expenseGroupFactory.CreateDataShapedObject(eg, lstOfFields)));
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult Get(int id, string fields = null)
        {
            try
            {
                bool includeExpenses = false;
                List<string> lstOfFields = new List<string>();

                // we should include expenses when the fields-string contains "expenses"
                if (fields != null)
                {
                    lstOfFields = fields.ToLower().Split(',').ToList();
                    includeExpenses = lstOfFields.Any(f => f.Contains("expenses"));
                }


                Repository.Entities.ExpenseGroup expenseGroup;
                if (includeExpenses)
                {
                    expenseGroup = _repository.GetExpenseGroupWithExpenses(id);
                }
                else
                {
                    expenseGroup = _repository.GetExpenseGroup(id);

                }


                if (expenseGroup != null)
                {
                    return Ok(_expenseGroupFactory.CreateDataShapedObject(expenseGroup, lstOfFields));
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // [FromBody] ExpenseGroup expenseGroup ==> Automatically try to convert FromBody to ExpenseGroup
        [HttpPost]
        [Route("api/expensegroups")]
        public IHttpActionResult Post([FromBody] ExpenseGroup expenseGroup)
        {
            try
            {
                if (expenseGroup == null)
                {
                    return BadRequest();
                }

                // try mapping & saving
                Repository.Entities.ExpenseGroup eg = _expenseGroupFactory.CreateExpenseGroup(expenseGroup);

                RepositoryActionResult<Repository.Entities.ExpenseGroup> result = _repository.InsertExpenseGroup(eg);
                if (result.Status == RepositoryActionStatus.Created)
                {
                    // map to dto
                    ExpenseGroup newExpenseGroup = _expenseGroupFactory.CreateExpenseGroup(result.Entity);
                    return Created(Request.RequestUri + "/" + newExpenseGroup.Id, newExpenseGroup);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }

            //{
            //    "userId":"https://expensetrackeridsrv3/embedded_1",
            //    "title":"New ExpenseGroup",
            //    "description":"ExpenseGroup description",
            //    "expenseGroupStatusId":1,
            //} returns id 19...
        }

        public IHttpActionResult Put(int id, [FromBody] ExpenseGroup expenseGroup)
        {
            try
            {
                if (expenseGroup == null)
                    return BadRequest();

                // map
                Repository.Entities.ExpenseGroup eg = _expenseGroupFactory.CreateExpenseGroup(expenseGroup);

                RepositoryActionResult<Repository.Entities.ExpenseGroup> result = _repository.UpdateExpenseGroup(eg);
                if (result.Status == RepositoryActionStatus.Updated)
                {
                    // map to dto
                    ExpenseGroup updatedExpenseGroup = _expenseGroupFactory
                        .CreateExpenseGroup(result.Entity);
                    return Ok(updatedExpenseGroup);
                }
                if (result.Status == RepositoryActionStatus.NotFound)
                {
                    return NotFound();
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }

            //{
            //    "id":19
            //    "userId":"https://expensetrackeridsrv3/embedded_1",
            //    "title":"New ExpenseGroup Updated",
            //    "description":"ExpenseGroup description",
            //    "expenseGroupStatusId":1,
            //} Full Body with all field filled is a must
        }


        // The PATCH standard ==> IETF: https://tools.ietf.org/html/rfc6902
        // This define a JSON Document for expressing a sequence of operations to apply
        [HttpPatch]
        public IHttpActionResult Patch(int id, [FromBody] JsonPatchDocument<ExpenseGroup> expenseGroupPatchDocument)
        {
            try
            {
                if (expenseGroupPatchDocument == null)
                {
                    return BadRequest();
                }

                Repository.Entities.ExpenseGroup expenseGroup = _repository.GetExpenseGroup(id);
                if (expenseGroup == null)
                {
                    return NotFound();
                }

                // map
                ExpenseGroup eg = _expenseGroupFactory.CreateExpenseGroup(expenseGroup);

                // apply changes to the DTO
                expenseGroupPatchDocument.ApplyTo(eg);

                // map the DTO with applied changes to the entity, & update
                RepositoryActionResult<Repository.Entities.ExpenseGroup> result =
                    _repository.UpdateExpenseGroup(_expenseGroupFactory.CreateExpenseGroup(eg));

                if (result.Status == RepositoryActionStatus.Updated)
                {
                    // map to dto
                    ExpenseGroup patchedExpenseGroup = _expenseGroupFactory.CreateExpenseGroup(result.Entity);
                    return Ok(patchedExpenseGroup);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }

            //[
            //    {"op":"replace","path":"/title","value":"New Title"},
            //    {"op":"copy","from":"/title","path":"/description"},
            //] Result
            //{
            //    //    "title":"New Title",
            //    //    "description":"New Title",
            //    //    "id":19,
            //    //    "userId":"https://expensetrackeridsrv3/embedded_1",
            //    //    "expenseGroupStatusId":1,
            //    //    "expenses": []
            //}
        }


        public IHttpActionResult Delete(int id)
        {
            try
            {
                RepositoryActionResult<Repository.Entities.ExpenseGroup> result = _repository.DeleteExpenseGroup(id);

                if (result.Status == RepositoryActionStatus.Deleted)
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
                if (result.Status == RepositoryActionStatus.NotFound)
                {
                    return NotFound();
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}