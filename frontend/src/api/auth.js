import axios from 'axios'

const API_BASE = 'http://localhost:5251/api'

const api = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' }
})

// 请求拦截器：自动带上 Token
api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// 响应拦截器：401 时跳转登录
api.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

// 登录接口
export function login(userName, password) {
  return api.post('/Auth/login', { userName, password })
}

// 获取用户信息（需要 Token）
export function getProfile() {
  return api.get('/Auth/profile')
}

// 获取用户菜单（需要 Token）
export function getMenus() {
  return api.get('/Auth/menus')
}
