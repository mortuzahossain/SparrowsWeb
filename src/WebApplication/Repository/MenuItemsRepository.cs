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
    public class MenuItemsRepository
    {
        public CommonResponse AddMenuItems(MenuItemViewModel menuItemViewModel)
        {

            try
            {
                string errMsg = string.Empty;
                int response = new ProcedureManager(DBName.Sparrows, ref errMsg).ExecuteNonQuery(ExecutionSP.sp_add_menu_item, ref errMsg, menuItemViewModel.IID, menuItemViewModel.Name, menuItemViewModel.DisplayName, menuItemViewModel.Details, menuItemViewModel.CategoryId,
                    menuItemViewModel.IsAvailableVariant, menuItemViewModel.IsAvailableAdons, menuItemViewModel.Price, menuItemViewModel.DiscountPrice, menuItemViewModel.Image, menuItemViewModel.Status, menuItemViewModel.StockPosition);
                if(response != 0)
                {
                    return new CommonResponse
                    {
                        ResponseCode = (int)ResponseCode.Success,
                        ResponseMsg = ResponseMessage.Success,
                        ResponseUserMsg = ResponseMessage.MenuItemAddSuccess
                    };
                }
                else
                {
                    return new CommonResponse
                    {
                        ResponseCode = (int)ResponseCode.OperationFailed,
                        ResponseMsg = "Failed to add menu item.",
                        ResponseUserMsg = ResponseMessage.MenuItemAddSuccess
                    };
                }

            }
            catch (Exception exception)
            {
                return new CommonResponse
                {
                    ResponseCode = (int)ResponseCode.OperationFailed,
                    ResponseMsg = exception.Message,
                    ResponseUserMsg = ResponseMessage.MenuItemAddSuccess
                };
            }

        }
        public CommonResponse GetAllMenuItems(string IID)
        {

            try
            {
                string errMsg = string.Empty;
                DataTable dt = (new ProcedureManager(DBName.Sparrows, ref errMsg)).ExecSPreturnDataTable(ExecutionSP.sp_Get_MenuItems, ref errMsg,IID);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {

                        List<MenuItemViewModel> models = new List<MenuItemViewModel>();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            MenuItemViewModel model = new MenuItemViewModel()
                            {
                                Id = dt.Rows[i]["Id"].ToString().Trim(),
                                Name = dt.Rows[i]["Name"].ToString().Trim(),
                                DisplayName = dt.Rows[i]["DisplayName"].ToString().Trim(),
                                StatusId = dt.Rows[i]["Status"].ToString().Trim(),
                                Details = dt.Rows[i]["Details"].ToString().Trim()//.Length >50?dt.Rows[i]["Details"].ToString().Trim().Substring(0, 100) : dt.Rows[i]["Details"].ToString().Trim()
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
        public CommonResponse GetMenuItemsById(string id)
        {

            try
            {
                string errMsg = string.Empty;
                DataTable dt = (new ProcedureManager(DBName.Sparrows, ref errMsg)).ExecSPreturnDataTable(ExecutionSP.sp_Get_MenuItemsById, ref errMsg, id);

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        MenuItemViewModel model = new MenuItemViewModel();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            model = new MenuItemViewModel()
                            {
                                Id = dt.Rows[i]["Id"].ToString().Trim(),
                                Name = dt.Rows[i]["Name"].ToString().Trim(),
                                DisplayName = dt.Rows[i]["DisplayName"].ToString().Trim(),
                                StatusId = dt.Rows[i]["Status"].ToString().Trim(),
                                Details = dt.Rows[i]["Details"].ToString().Trim(),
                                Image = dt.Rows[i]["Image"].ToString().Trim(),
                                Price = dt.Rows[i]["Price"].ToString().Trim(),
                                CategoryId = dt.Rows[i]["CategoryId"].ToString().Trim(),
                                DiscountPrice = dt.Rows[i]["DiscountPrice"].ToString().Trim(),
                                IsAvailableAdonsId = dt.Rows[i]["IsAvailableAdons"].ToString().Trim(),
                                IsAvailableVariantId = dt.Rows[i]["IsAvailableVariant"].ToString().Trim(),
                                StockPositionId = dt.Rows[i]["StockPosition"].ToString().Trim(),
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
        public CommonResponse UpdateMenuItems(MenuItemViewModel menuItemView)
        {

            try
            {
                string errMsg = string.Empty;
                int response = new ProcedureManager(DBName.Sparrows, ref errMsg).ExecuteNonQuery(ExecutionSP.sp_up_menu_items, ref errMsg, menuItemView.Id,menuItemView.Name,menuItemView.DisplayName, menuItemView.Details, menuItemView.CategoryId, menuItemView.IsAvailableVariant,menuItemView.IsAvailableAdons, menuItemView.Price, menuItemView.DiscountPrice, menuItemView.Image, menuItemView.Status, menuItemView.StockPosition);
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
                        ResponseMsg = "Failed to update menu items.",
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
        public CommonResponse DeleteMenuItems(string Id)
        {

            try
            {
                string errMsg = string.Empty;
                Int32 response = new ProcedureManager(DBName.Sparrows, ref errMsg).ExecuteNonQuery(ExecutionSP.sp_dl_menu_items, ref errMsg, Id);
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