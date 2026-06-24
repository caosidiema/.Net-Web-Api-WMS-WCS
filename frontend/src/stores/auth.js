import { defineStore } from 'pinia'
import { ref } from 'vue'
import { login as loginApi, getMenus } from '../api/auth'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || '')
  const userName = ref(localStorage.getItem('userName') || '')
  const isLoggedIn = ref(!!token.value)
  const menus = ref([])  // 动态菜单

  async function login(form) {
    const res = await loginApi(form.userName, form.password)
    token.value = res.data.token
    userName.value = form.userName
    isLoggedIn.value = true
    localStorage.setItem('token', res.data.token)
    localStorage.setItem('userName', form.userName)
    // 登录成功后获取菜单
    await fetchMenus()
  }

  // 获取动态菜单
  async function fetchMenus() {
    try {
      const res = await getMenus()
      menus.value = res.data || []
    } catch (err) {
      console.error('获取菜单失败:', err)
      menus.value = []
    }
  }

  function logout() {
    token.value = ''
    userName.value = ''
    isLoggedIn.value = false
    menus.value = []
    localStorage.removeItem('token')
    localStorage.removeItem('userName')
  }

  return { token, userName, isLoggedIn, menus, login, logout, fetchMenus }
})
