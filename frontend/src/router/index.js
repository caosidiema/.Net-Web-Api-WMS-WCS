import { createRouter, createWebHistory } from 'vue-router'
import Login from '../views/Login.vue'
import Layout from '../components/layout/Layout.vue'
import Home from '../views/Home.vue'
import Dashboard from '../views/Dashboard.vue'

const routes = [
  { path: '/', redirect: '/home' },
  { path: '/login', name: 'Login', component: Login },
  {
    path: '/home',
    component: Layout,
    redirect: '/home/index',
    meta: { requiresAuth: true },
    children: [
      // 首页仪表盘
      { path: 'index', name: 'Dashboard', component: Dashboard, meta: { title: '控制台' } },

      // 库存管理
      { path: 'inventory/query', name: 'InventoryQuery', component: () => import('../views/inventory/InventoryQuery.vue'), meta: { title: '库存查询' } },
      { path: 'inventory/warning', name: 'InventoryWarning', component: () => import('../views/inventory/InventoryWarning.vue'), meta: { title: '库存预警' } },
      { path: 'inventory/check', name: 'InventoryCheck', component: () => import('../views/inventory/InventoryCheck.vue'), meta: { title: '库存盘点' } },

      // 出入库管理
      { path: 'inbound/list', name: 'InboundList', component: () => import('../views/inbound/InboundList.vue'), meta: { title: '入库列表' } },
      { path: 'outbound/list', name: 'OutboundList', component: () => import('../views/outbound/OutboundList.vue'), meta: { title: '出库列表' } },
      { path: 'flow/record', name: 'FlowRecord', component: () => import('../views/flow/FlowRecord.vue'), meta: { title: '流转记录' } },

      // 系统管理
      { path: 'system/users', name: 'UserManage', component: () => import('../views/system/UserManage.vue'), meta: { title: '用户管理' } },
      { path: 'system/roles', name: 'RoleManage', component: () => import('../views/system/RoleManage.vue'), meta: { title: '角色管理' } }
    ]
  },

  // 404 兜底
  { path: '/:pathMatch(.*)*', redirect: '/home' }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// 路由守卫：未登录跳转到登录页
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')
  if (to.meta.requiresAuth && !token) {
    next('/login')
  } else {
    next()
  }
})

export default router
