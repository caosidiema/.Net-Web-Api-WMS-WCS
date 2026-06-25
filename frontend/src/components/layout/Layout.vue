<template>
  <el-container class="layout-container">
    <!-- 侧边栏 -->
    <el-aside :width="collapsed ? '64px' : '220px'" class="sidebar">
      <!-- Logo 区域 -->
      <div class="sidebar-logo">
        <div class="logo-icon">
          <svg viewBox="0 0 24 24" fill="none">
            <path d="M20 7L12 3L4 7M20 7V17L12 21M20 7L12 11M12 11L4 7M12 11V21M4 7V17L12 21" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </div>
        <span v-if="!collapsed" class="logo-text">WMS</span>
      </div>

      <!-- 菜单 -->
      <el-menu
        :default-active="currentRoute"
        :collapse="collapsed"
        :collapse-transition="false"
        class="sidebar-menu"
        background-color="#1e293b"
        text-color="rgba(255,255,255,0.65)"
        active-text-color="#ffffff"
        @select="handleMenuSelect"
      >
        <template v-for="menu in menus" :key="menu.path">
          <!-- 有子菜单 -->
          <el-sub-menu v-if="menu.children && menu.children.length" :index="menu.path">
            <template #title>
              <el-icon><component :is="getIconComponent(menu.icon)" /></el-icon>
              <span>{{ menu.name }}</span>
            </template>
            <el-menu-item
              v-for="child in menu.children"
              :key="child.path"
              :index="child.path"
            >
              <el-icon><component :is="getIconComponent(child.icon)" /></el-icon>
              <span>{{ child.name }}</span>
            </el-menu-item>
          </el-sub-menu>
          <!-- 无子菜单 -->
          <el-menu-item v-else :index="menu.path">
            <el-icon><component :is="getIconComponent(menu.icon)" /></el-icon>
            <span>{{ menu.name }}</span>
          </el-menu-item>
        </template>
      </el-menu>
    </el-aside>

    <!-- 主体区域 -->
    <el-container>
      <!-- 头部 -->
      <el-header class="layout-header">
        <div class="header-left">
          <el-icon class="collapse-btn" @click="collapsed = !collapsed">
            <Fold v-if="!collapsed" />
            <Expand v-else />
          </el-icon>
          <el-breadcrumb separator="/">
            <el-breadcrumb-item :to="{ path: '/home' }">首页</el-breadcrumb-item>
            <el-breadcrumb-item v-if="currentMenuName">{{ currentMenuName }}</el-breadcrumb-item>
          </el-breadcrumb>
        </div>
        <div class="header-right">
          <el-badge :value="3" :max="99" class="header-icon-btn">
            <el-icon :size="20"><Bell /></el-icon>
          </el-badge>
          <el-dropdown @command="handleCommand">
            <div class="user-info">
              <el-avatar :size="32" class="user-avatar">
                {{ authStore.userName?.charAt(0)?.toUpperCase() || 'A' }}
              </el-avatar>
              <span class="user-name">{{ authStore.userName || '管理员' }}</span>
              <el-icon><ArrowDown /></el-icon>
            </div>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item command="profile">个人中心</el-dropdown-item>
                <el-dropdown-item command="settings">系统设置</el-dropdown-item>
                <el-dropdown-item divided command="logout">退出登录</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </el-header>

      <!-- 内容区 -->
      <el-main class="layout-main">
        <!-- 标签页 -->
        <div class="tags-nav">
          <div class="tags-list">
            <el-tag
              v-for="tag in visitedRoutes"
              :key="tag.path"
              :type="tag.path === currentRoute ? '' : 'info'"
              :closable="tag.closable !== false"
              @click="router.push(tag.path)"
              @close="handleCloseTag(tag)"
              class="tag-item"
            >
              {{ tag.meta?.title || tag.name }}
            </el-tag>
          </div>
          <el-dropdown @command="handleTagsAction" class="tags-actions">
            <el-icon><MoreFilled /></el-icon>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item command="closeOthers">关闭其他</el-dropdown-item>
                <el-dropdown-item command="closeAll">关闭全部</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>

        <!-- 路由内容 -->
        <div class="content-wrapper">
          <router-view v-slot="{ Component }">
            <transition name="fade-transform" mode="out-in">
              <keep-alive>
                <component :is="Component" />
              </keep-alive>
            </transition>
          </router-view>
        </div>
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import {
  Fold, Expand, Bell, ArrowDown, MoreFilled,
  DataBoard, Box, ShoppingCart, Van, Setting, User,
  Odometer, Goods, List, Location, Tools, Avatar
} from '@element-plus/icons-vue'
import { useAuthStore } from '../../stores/auth'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const collapsed = ref(false)
const menus = computed(() => authStore.menus)
const currentRoute = computed(() => route.path)

// 获取当前菜单名称
const currentMenuName = computed(() => {
  for (const menu of menus.value) {
    if (menu.children?.some(child => child.path === currentRoute.value)) {
      const child = menu.children.find(child => child.path === currentRoute.value)
      return child?.name
    }
    if (menu.path === currentRoute.value) {
      return menu.name
    }
  }
  return ''
})

// 访问过的路由（用于标签页）
const visitedRoutes = ref([])

// 初始化标签页
function initTags() {
  const defaultRoute = {
    path: '/home',
    name: 'Dashboard',
    meta: { title: '控制台' },
    closable: false
  }
  visitedRoutes.value = [defaultRoute]
}

// 添加标签页
function addTag(route) {
  if (route.path === '/home') return
  const exists = visitedRoutes.value.find(t => t.path === route.path)
  if (!exists) {
    visitedRoutes.value.push({
      path: route.path,
      name: route.name,
      meta: route.meta,
      closable: true
    })
  }
}

// 关闭标签
function handleCloseTag(tag) {
  const index = visitedRoutes.value.findIndex(t => t.path === tag.path)
  visitedRoutes.value.splice(index, 1)
  if (tag.path === currentRoute.value) {
    const nextTag = visitedRoutes.value[index - 1] || visitedRoutes.value[0]
    router.push(nextTag.path)
  }
}

// 标签页操作
function handleTagsAction(command) {
  if (command === 'closeOthers') {
    visitedRoutes.value = visitedRoutes.value.filter(t => !t.closable || t.path === currentRoute.value)
  } else if (command === 'closeAll') {
    visitedRoutes.value = visitedRoutes.value.filter(t => !t.closable)
    router.push('/home')
  }
}

// 菜单选择
function handleMenuSelect(path) {
  router.push(path)
}

// 用户操作
function handleCommand(command) {
  if (command === 'logout') {
    authStore.logout()
    ElMessage.success('已退出登录')
    router.push('/login')
  } else if (command === 'profile') {
    router.push('/home/profile')
  } else if (command === 'settings') {
    router.push('/home/settings')
  }
}

// 图标映射
function getIconComponent(iconName) {
  const iconMap = {
    'DataBoard': DataBoard,
    'Odometer': Odometer,
    'Box': Box,
    'Goods': Goods,
    'ShoppingCart': ShoppingCart,
    'List': List,
    'Van': Van,
    'Location': Location,
    'Setting': Setting,
    'Tools': Tools,
    'User': User,
    'Avatar': Avatar
  }
  return iconMap[iconName] || DataBoard
}

// 监听路由变化添加标签
watch(() => route.path, (newPath) => {
  addTag(route)
})

// 初始化
onMounted(async () => {
  initTags()
  addTag(route)
  if (authStore.token && menus.value.length === 0) {
    await authStore.fetchMenus()
  }
})
</script>

<style scoped>
.layout-container {
  height: 100vh;
  background: #f0f2f5;
}

/* 侧边栏 */
.sidebar {
  background: #1e293b;
  transition: width 0.3s ease;
  overflow: hidden;
}

.sidebar-logo {
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
}

.logo-icon {
  width: 32px;
  height: 32px;
  color: #667eea;
  flex-shrink: 0;
}

.logo-icon svg {
  width: 100%;
  height: 100%;
}

.logo-text {
  font-size: 18px;
  font-weight: 700;
  color: white;
  letter-spacing: 2px;
}

.sidebar-menu {
  border-right: none;
  background: transparent;
}

.sidebar-menu:not(.el-menu--collapse) {
  width: 220px;
}

.sidebar-menu .el-menu-item,
.sidebar-menu .el-sub-menu__title {
  height: 50px;
  line-height: 50px;
  margin: 4px 8px;
  border-radius: 8px;
}

.sidebar-menu .el-menu-item:hover,
.sidebar-menu .el-sub-menu__title:hover {
  background: rgba(255, 255, 255, 0.08);
}

.sidebar-menu .el-menu-item.is-active {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.sidebar-menu .el-sub-menu .el-menu-item {
  height: 46px;
  line-height: 46px;
  margin: 2px 8px;
  padding-left: 52px !important;
}

/* 头部 */
.layout-header {
  background: white;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 20px;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.04);
  height: 60px;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 16px;
}

.collapse-btn {
  font-size: 20px;
  cursor: pointer;
  color: #666;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.2s;
}

.collapse-btn:hover {
  background: #f5f5f5;
  color: #333;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 16px;
}

.header-icon-btn {
  cursor: pointer;
  padding: 8px;
  border-radius: 8px;
  transition: background 0.2s;
}

.header-icon-btn:hover {
  background: #f5f5f5;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  padding: 6px 12px;
  border-radius: 8px;
  transition: background 0.2s;
}

.user-info:hover {
  background: #f5f5f5;
}

.user-avatar {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  font-weight: 600;
}

.user-name {
  font-size: 14px;
  font-weight: 500;
  color: #333;
}

/* 内容区 */
.layout-main {
  padding: 0;
  display: flex;
  flex-direction: column;
}

/* 标签页 */
.tags-nav {
  height: 40px;
  background: white;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 16px;
  border-bottom: 1px solid #f0f0f0;
}

.tags-list {
  display: flex;
  align-items: center;
  gap: 8px;
  overflow-x: auto;
  flex: 1;
}

.tags-list::-webkit-scrollbar {
  height: 0;
}

.tag-item {
  cursor: pointer;
  white-space: nowrap;
  transition: all 0.2s;
}

.tag-item:hover {
  opacity: 0.8;
}

.tags-actions {
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
}

.tags-actions:hover {
  background: #f5f5f5;
}

/* 内容区域 */
.content-wrapper {
  flex: 1;
  padding: 16px;
  overflow-y: auto;
}

/* 路由过渡动画 */
.fade-transform-enter-active,
.fade-transform-leave-active {
  transition: all 0.3s ease;
}

.fade-transform-enter-from {
  opacity: 0;
  transform: translateX(20px);
}

.fade-transform-leave-to {
  opacity: 0;
  transform: translateX(-20px);
}
</style>
