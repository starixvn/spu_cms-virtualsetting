// -----------------------------------------------------------------------
// <copyright file="QueryService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NAP.Framework.Command;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Base Query Service class.
    /// </summary>
    public class QueryService
    {
        private readonly IDynamicRepository _repository;

        public QueryService(IDynamicRepository repository = null)
        {
            _repository = repository ?? new DynamicRepository();
        }

        #region GET-DATA

        public virtual IEnumerable<T> GetData<T>(ICriteria criteria)
        {
            return _repository.Fetch<T>(criteria);
        }

        public virtual IEnumerable<T> GetData<T>(dynamic paramaters)
        {
            return _repository.Query<T>(paramaters);
        }

        public virtual T Get<T>(ICriteria criteria)
        {
            return _repository.Get<T>(criteria);
        }

        /// <summary>
        ///     Get Data by template.
        /// </summary>
        /// <typeparam name="T">Delegate object.</typeparam>
        /// <param name="criteria">Input criteria.</param>
        /// <param name="totalPages">Total Pages.</param>
        /// <returns>List of delegate objects.</returns>
        public IEnumerable<T> GetData<T>(ICriteria criteria, ref int totalPages)
        {
            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var param = criteria.GetSpParams();
                var list = objectDb.Query<T>(param);
                totalPages = Convert.ToInt32(param["totalpages"]);

                return list;
            }
        }

        #endregion

        #region UPDATE-DATA

        public ICommandResult ExecuteOutput(ICriteria criteria)
        {
            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var param = criteria.GetSpParams();
                objectDb.ExecuteNonQuery(param);

                return GetCommandResult(param);
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        ///     Get Command Result.
        /// </summary>
        /// <param name="hash">Input parameters.</param>
        /// <returns>Command result.</returns>
        public CommandResult GetCommandResult(Hashtable hash)
        {
            var errCode = hash["errcode"];
            var errMsg = hash["errmsg"];

            return new CommandResult
            {
                Code = (errCode is DBNull) ? 0 : Convert.ToInt32(errCode),
                Message = (errMsg is DBNull) ? string.Empty : Convert.ToString(errMsg)
            };
        }
        #endregion
    }
}
