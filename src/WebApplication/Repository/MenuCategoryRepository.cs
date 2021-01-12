using DBManager;
using GlobalEntities.Entities;
using GlobalEntities.Variables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WebApplication.Models;
using static GlobalEntities.Enums.GlobalEnums;

namespace WebApplication.Repository
{
    public class MenuCategoryRepository
    {
        public CommonResponse AddMenuCategory(MenuCategoryViewModel menuCategoryViewModel)
        {

            try
            {
                string errMsg = string.Empty;
                Int32 response = new ProcedureManager(DBName.Sparrows, ref errMsg).ExecuteNonQuery(ExecutionSP.sp_add_menu_category, ref errMsg, menuCategoryViewModel.IID, menuCategoryViewModel.Name, menuCategoryViewModel.DisplayName, menuCategoryViewModel.Details, menuCategoryViewModel.Status, menuCategoryViewModel.Image);
                if(response != 0)
                {
                    return new CommonResponse
                    {
                        ResponseCode = (int)ResponseCode.Success,
                        ResponseMsg = ResponseMessage.Success,
                        ResponseUserMsg = ResponseMessage.MenuCategoryAddSuccess
                    };
                }
                else
                {
                    return new CommonResponse
                    {
                        ResponseCode = (int)ResponseCode.OperationFailed,
                        ResponseMsg = "Failed to add menu categoty.",
                        ResponseUserMsg = ResponseMessage.MenuAddFailed
                    };
                }

            }
            catch (Exception exception)
            {
                return new CommonResponse
                {
                    ResponseCode = (int)ResponseCode.OperationFailed,
                    ResponseMsg = exception.Message,
                    ResponseUserMsg = ResponseMessage.MenuAddFailed
                };
            }

        }
        public CommonResponse GetAllMenuCategory(string IID)
        {

            try
            {
                string errMsg = string.Empty;
                DataTable dt = (new ProcedureManager(DBName.Sparrows, ref errMsg)).ExecSPreturnDataTable(ExecutionSP.sp_Get_MenuCategory, ref errMsg,IID);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {

                        List<MenuCategoryViewModel> models = new List<MenuCategoryViewModel>();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            MenuCategoryViewModel model = new MenuCategoryViewModel()
                            {
                                Id = dt.Rows[i]["Id"].ToString().Trim(),
                                Name = dt.Rows[i]["Name"].ToString().Trim(),
                                DisplayName = dt.Rows[i]["DisplayName"].ToString().Trim(),
                                StatusId = dt.Rows[i]["Status"].ToString().Trim(),
                                Details = dt.Rows[i]["Details"].ToString().Trim()
                            };
                           
                            models.Add(model);
                        }

                        return new CommonResponse
                        {
                            ResponseCode = (int)ResponseCode.Success,
                            ResponseMsg = ResponseMessage.Success,
                            ResponseUserMsg = ResponseMessage.GetMenuCategorySuccess,
                            ResponseData = models
                        };

                    }
                    else
                    {
                        return new CommonResponse
                        {
                            ResponseCode = (int)ResponseCode.OperationFailed,
                            ResponseMsg = "Failed to get menu categoty.",
                            ResponseUserMsg = ResponseMessage.GetMenuCategoryFailed
                        };

                    }
                }
                else
                {
                    return new CommonResponse
                    {
                        ResponseCode = (int)ResponseCode.OperationFailed,
                        ResponseMsg = "Failed to get menu categoty.",
                        ResponseUserMsg = ResponseMessage.GetMenuCategoryFailed
                    };
                }


            }
            catch (Exception exception)
            {
                return new CommonResponse
                {
                    ResponseCode = (int)ResponseCode.OperationFailed,
                    ResponseMsg = exception.Message,
                    ResponseUserMsg = ResponseMessage.GetMenuCategoryFailed
                };
            }

        }
        public CommonResponse GetAllMenuCategoryById(string id)
        {

            try
            {
                string errMsg = string.Empty;
                DataTable dt = (new ProcedureManager(DBName.Sparrows, ref errMsg)).ExecSPreturnDataTable(ExecutionSP.sp_Get_MenuCategoryById, ref errMsg, id);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        MenuCategoryViewModel model = new MenuCategoryViewModel();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            model = new MenuCategoryViewModel()
                            {
                                Id = dt.Rows[i]["Id"].ToString().Trim(),
                                Name = dt.Rows[i]["Name"].ToString().Trim(),
                                DisplayName = dt.Rows[i]["DisplayName"].ToString().Trim(),
                                StatusId = dt.Rows[i]["Status"].ToString().Trim(),
                                Details = dt.Rows[i]["Details"].ToString().Trim(),
                                Image = dt.Rows[i]["Image"].ToString().Trim()
                            };

                        }

                        return new CommonResponse
                        {
                            ResponseCode = (int)ResponseCode.Success,
                            ResponseMsg = ResponseMessage.Success,
                            ResponseUserMsg = ResponseMessage.GetMenuCategorySuccess,
                            ResponseData = model
                        };

                    }
                    else
                    {
                        return new CommonResponse
                        {
                            ResponseCode = (int)ResponseCode.OperationFailed,
                            ResponseMsg = "Failed to get menu categoty.",
                            ResponseUserMsg = ResponseMessage.GetMenuCategoryFailed
                        };

                    }
                }
                else
                {
                    return new CommonResponse
                    {
                        ResponseCode = (int)ResponseCode.OperationFailed,
                        ResponseMsg = "Failed to get menu categoty.",
                        ResponseUserMsg = ResponseMessage.GetMenuCategoryFailed
                    };
                }


            }
            catch (Exception exception)
            {
                return new CommonResponse
                {
                    ResponseCode = (int)ResponseCode.OperationFailed,
                    ResponseMsg = exception.Message,
                    ResponseUserMsg = ResponseMessage.GetMenuCategoryFailed
                };
            }

        }

        public CommonResponse UpdateMenuCategory(MenuCategoryViewModel menuCategoryViewModel)
        {

            try
            {
                string errMsg = string.Empty;
                Int32 response = new ProcedureManager(DBName.Sparrows, ref errMsg).ExecuteNonQuery(ExecutionSP.sp_up_menu_category, ref errMsg, menuCategoryViewModel.Id, menuCategoryViewModel.Name, menuCategoryViewModel.DisplayName, menuCategoryViewModel.Details, menuCategoryViewModel.Status, menuCategoryViewModel.Image);
                if (response != 0)
                {
                    return new CommonResponse
                    {
                        ResponseCode = (int)ResponseCode.Success,
                        ResponseMsg = ResponseMessage.Success,
                        ResponseUserMsg = ResponseMessage.MenuCategoryAddSuccess
                    };
                }
                else
                {
                    return new CommonResponse
                    {
                        ResponseCode = (int)ResponseCode.OperationFailed,
                        ResponseMsg = "Failed to add menu categoty.",
                        ResponseUserMsg = ResponseMessage.MenuAddFailed
                    };
                }

            }
            catch (Exception exception)
            {
                return new CommonResponse
                {
                    ResponseCode = (int)ResponseCode.OperationFailed,
                    ResponseMsg = exception.Message,
                    ResponseUserMsg = ResponseMessage.MenuAddFailed
                };
            }

        }
        public CommonResponse DeleteMenuCategory(string Id)
        {

            try
            {
                string errMsg = string.Empty;
                Int32 response = new ProcedureManager(DBName.Sparrows, ref errMsg).ExecuteNonQuery(ExecutionSP.sp_dl_menu_category, ref errMsg, Id);
                if (response != 0)
                {
                    return new CommonResponse
                    {
                        ResponseCode = (int)ResponseCode.Success,
                        ResponseMsg = ResponseMessage.Success,
                        ResponseUserMsg = ResponseMessage.MenuCategoryAddSuccess
                    };
                }
                else
                {
                    return new CommonResponse
                    {
                        ResponseCode = (int)ResponseCode.OperationFailed,
                        ResponseMsg = "Failed to add menu categoty.",
                        ResponseUserMsg = ResponseMessage.MenuAddFailed
                    };
                }

            }
            catch (Exception exception)
            {
                return new CommonResponse
                {
                    ResponseCode = (int)ResponseCode.OperationFailed,
                    ResponseMsg = exception.Message,
                    ResponseUserMsg = ResponseMessage.MenuAddFailed
                };
            }

        }
    }
}