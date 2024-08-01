// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const API_URL = "https://localhost:44383";
const HOST_URL = "http://localhost:5011";

const PRODUCT_FILTER_API = `${API_URL}/api/GetListProduct/Process`;
const PRODUCT_SELECT_API = `${API_URL}/api/`;
const PRODUCT_STOCK_API = (id) => `${API_URL}/api/products/${id}/stock`;

const ORDER_SUBMIT_API = `${API_URL}/api/orders/create`;
const SHOW_ORDER_API = (id) => `${HOST_URL}/orders/${id}/view`;
const ORDER_ADD_ITEM = `${HOST_URL}/orders/add-item`;
const ORDER_UPDATE_ITEM_QUANTITY = (id, quantity) => `${HOST_URL}/orders/items/${id}?quantity=${quantity}`;
const ORDER_REMOVE_ITEM = (id) => `${HOST_URL}/orders/items/${id}`;
const ORDER_CHECKOUT_API = `${HOST_URL}/orders/checkout`;
const ORDER_UPDATE_API = `${HOST_URL}/orders/update`;
const ORDER_CLEAR_API = `${HOST_URL}/orders/clear`;
const ORDER_TEMP_SAVE_API = `${HOST_URL}/orders/save-to-session`;

const GET_BASIC_CUSTOMER_INFO = (phone) => `${HOST_URL}/orders/customers/${phone}`;