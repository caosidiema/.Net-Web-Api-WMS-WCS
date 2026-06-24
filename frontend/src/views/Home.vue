<template>
  <div class="home-container">
    <!-- 侧边栏 -->
    <aside class="sidebar" :class="{ collapsed: sidebarCollapsed }">
      <div class="sidebar-header">
        <div class="sidebar-logo">
          <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M20 7L12 3L4 7M20 7V17L12 21M20 7L12 11M12 11L4 7M12 11V21M4 7V17L12 21" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          <span v-if="!sidebarCollapsed" class="logo-text">WMS</span>
        </div>
      </div>

      <nav class="sidebar-nav">
        <template v-for="menu in menus" :key="menu.path">
          <div class="nav-section" v-if="menu.children && menu.children.length">
            <div class="nav-section-title" v-if="!sidebarCollapsed">{{ menu.name }}</div>
            <div class="nav-item" 
                 v-for="child in menu.children" 
                 :key="child.path"
                 :class="{ active: currentPath === child.path }"
                 @click="navigateTo(child.path)">
              <el-icon><component :is="getIconComponent(child.icon)" /></el-icon>
              <span v-if="!sidebarCollapsed">{{ child.name }}</span>
            </div>
          </div>
          <div class="nav-section" v-else>
            <div class="nav-item"
                 :class="{ active: currentPath === menu.path }"
                 @click="navigateTo(menu.path)">
              <el-icon><component :is="getIconComponent(menu.icon)" /></el-icon>
              <span v-if="!sidebarCollapsed">{{ menu.name }}</span>
            </div>
          </div>
        </template>
      </nav>

      <div class="sidebar-footer">
        <div class="nav-item" @click="handleLogout">
          <el-icon><SwitchButton /></el-icon>
          <span v-if="!sidebarCollapsed">退出登录</span>
        </div>
      </div>
    </aside>

    <!-- 主内容区 -->
    <main class="main-content">
      <!-- 顶部导航 -->
      <header class="top-header">
        <div class="header-left">
          <el-icon class="collapse-btn" @click="sidebarCollapsed = !sidebarCollapsed">
            <Fold v-if="!sidebarCollapsed" />
            <Expand v-else />
          </el-icon>
          <div class="breadcrumb">
            <span>首页</span>
            <el-icon><ArrowRight /></el-icon>
            <span>控制台</span>
          </div>
        </div>
        <div class="header-right">
          <div class="header-icon-btn">
            <el-badge :value="3" :max="99">
              <el-icon><Bell /></el-icon>
            </el-badge>
          </div>
          <div class="user-info">
            <el-avatar :size="36" class="user-avatar">
              {{ authStore.userName?.charAt(0)?.toUpperCase() || 'A' }}
            </el-avatar>
            <span class="user-name">{{ authStore.userName || 'Admin' }}</span>
          </div>
        </div>
      </header>

      <!-- 内容区域 -->
      <div class="content-area">
        <!-- 欢迎区域 -->
        <div class="welcome-section">
          <div class="welcome-text">
            <h1>欢迎回来，{{ authStore.userName || '管理员' }} 👋</h1>
            <p>今天是 {{ currentDate }}，这是你的仓储运营概览。</p>
          </div>
          <div class="welcome-actions">
            <el-button type="primary" @click="fetchProfile" :loading="loading">
              <el-icon><Refresh /></el-icon>
              刷新数据
            </el-button>
          </div>
        </div>

        <!-- 统计卡片 -->
        <div class="stats-grid">
          <div class="stat-card" v-for="stat in statsCards" :key="stat.title">
            <div class="stat-icon" :style="{ background: stat.color }">
              <el-icon :size="24"><component :is="stat.icon" /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-value">{{ stat.value }}</div>
              <div class="stat-title">{{ stat.title }}</div>
              <div class="stat-trend" :class="stat.trendType">
                <el-icon><Top v-if="stat.trendType === 'up'" /><Bottom v-else /></el-icon>
                <span>{{ stat.trend }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- 图表与活动区域 -->
        <div class="dashboard-grid">
          <!-- 库存概览 -->
          <div class="dashboard-card chart-card">
            <div class="card-header">
              <h3>库存概览</h3>
              <el-radio-group v-model="chartPeriod" size="small">
                <el-radio-button label="week">本周</el-radio-button>
                <el-radio-button label="month">本月</el-radio-button>
              </el-radio-group>
            </div>
            <div class="chart-placeholder">
              <div class="chart-bars">
                <div class="chart-bar" v-for="item in chartData" :key="item.label">
                  <div class="bar-fill" :style="{ height: item.value + '%' }"></div>
                  <div class="bar-label">{{ item.label }}</div>
                </div>
              </div>
            </div>
          </div>

          <!-- 最近活动 -->
          <div class="dashboard-card activity-card">
            <div class="card-header">
              <h3>最近活动</h3>
              <el-button text type="primary" size="small">查看全部</el-button>
            </div>
            <div class="activity-list">
              <div class="activity-item" v-for="(activity, index) in activities" :key="index">
                <div class="activity-dot" :style="{ background: activity.color }"></div>
                <div class="activity-content">
                  <div class="activity-text">{{ activity.text }}</div>
                  <div class="activity-time">{{ activity.time }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- 快捷操作 & 仓库状态 -->
        <div class="dashboard-grid">
          <div class="dashboard-card">
            <div class="card-header">
              <h3>快捷操作</h3>
            </div>
            <div class="quick-actions">
              <div class="action-btn" v-for="action in quickActions" :key="action.label">
                <div class="action-icon" :style="{ background: action.color }">
                  <el-icon :size="20"><component :is="action.icon" /></el-icon>
                </div>
                <span class="action-label">{{ action.label }}</span>
              </div>
            </div>
          </div>

          <div class="dashboard-card">
            <div class="card-header">
              <h3>仓库状态</h3>
            </div>
            <div class="warehouse-status">
              <div class="status-item" v-for="wh in warehouses" :key="wh.name">
                <div class="status-header">
                  <span class="status-name">{{ wh.name }}</span>
                  <span class="status-percent">{{ wh.usage }}%</span>
                </div>
                <el-progress :percentage="wh.usage" :color="getProgressColor(wh.usage)" :stroke-width="8" :show-text="false" />
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import {
  DataBoard, Box, ShoppingCart, Van, Setting, User,
  SwitchButton, Fold, Expand, ArrowRight, Bell, Refresh,
  Top, Bottom, Plus, Download, Upload, Search
} from '@element-plus/icons-vue'
import { useAuthStore } from '../stores/auth'
import { getProfile } from '../api/auth'

const router = useRouter()
const authStore = useAuthStore()
const loading = ref(false)
const sidebarCollapsed = ref(false)
const chartPeriod = ref('week')
const currentPath = ref('/home')
const menus = computed(() => authStore.menus)

// 初始化时获取菜单
onMounted(async () => {
  if (authStore.token && menus.value.length === 0) {
    await authStore.fetchMenus()
  }
})

// 路由跳转
function navigateTo(path) {
  currentPath.value = path
  router.push(path)
}

// 动态图标映射
function getIconComponent(iconName) {
  const iconMap = {
    'DataBoard': DataBoard,
    'Box': Box,
    'ShoppingCart': ShoppingCart,
    'Van': Van,
    'Setting': Setting,
    'User': User,
    'Odometer': DataBoard,
    'Goods': Box,
    'List': ShoppingCart,
    'Location': Van,
    'Tools': Setting,
    'Avatar': User
  }
  return iconMap[iconName] || DataBoard
}

const currentDate = computed(() => {
  const now = new Date()
  const options = { year: 'numeric', month: 'long', day: 'numeric', weekday: 'long' }
  return now.toLocaleDateString('zh-CN', options)
})

const statsCards = ref([
  { title: '总库存量', value: '12,846', icon: 'Box', color: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)', trend: '+12.5%', trendType: 'up' },
  { title: '今日入库', value: '328', icon: 'Download', color: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)', trend: '+8.2%', trendType: 'up' },
  { title: '今日出库', value: '256', icon: 'Upload', color: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)', trend: '-3.1%', trendType: 'down' },
  { title: '待处理订单', value: '42', icon: 'ShoppingCart', color: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)', trend: '+5.4%', trendType: 'up' }
])

const chartData = ref([
  { label: '周一', value: 65 },
  { label: '周二', value: 78 },
  { label: '周三', value: 52 },
  { label: '周四', value: 91 },
  { label: '周五', value: 84 },
  { label: '周六', value: 45 },
  { label: '周日', value: 30 }
])

const activities = ref([
  { text: '订单 #20240610-001 已发货', time: '10分钟前', color: '#67c23a' },
  { text: '新入库商品 128 件（A区-03架）', time: '32分钟前', color: '#409eff' },
  { text: '库存预警：SKU-8842 低于安全库存', time: '1小时前', color: '#e6a23c' },
  { text: '订单 #20240610-002 已确认', time: '2小时前', color: '#67c23a' },
  { text: '仓库B区完成盘点', time: '3小时前', color: '#909399' },
  { text: '退货单 RT-0056 已处理', time: '5小时前', color: '#f56c6c' }
])

const quickActions = ref([
  { label: '新建入库', icon: 'Plus', color: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)' },
  { label: '新建出库', icon: 'Upload', color: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)' },
  { label: '库存查询', icon: 'Search', color: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)' },
  { label: '导出报表', icon: 'Download', color: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)' }
])

const warehouses = ref([
  { name: 'A区 - 电子产品', usage: 78 },
  { name: 'B区 - 日用百货', usage: 56 },
  { name: 'C区 - 食品饮料', usage: 92 },
  { name: 'D区 - 服装鞋帽', usage: 34 }
])

function getProgressColor(usage) {
  if (usage >= 90) return '#f56c6c'
  if (usage >= 70) return '#e6a23c'
  return '#67c23a'
}

async function fetchProfile() {
  loading.value = true
  try {
    const res = await getProfile()
    ElMessage.success('数据已刷新')
  } catch (err) {
    ElMessage.error('刷新失败，请检查 Token')
  } finally {
    loading.value = false
  }
}

function handleLogout() {
  authStore.logout()
  ElMessage.success('已退出登录')
  router.push('/login')
}
</script>

<style scoped>
.home-container {
  display: flex;
  height: 100vh;
  background: #f0f2f5;
}

/* 侧边栏 */
.sidebar {
  width: 240px;
  background: #1e293b;
  display: flex;
  flex-direction: column;
  transition: width 0.3s ease;
  overflow: hidden;
}

.sidebar.collapsed {
  width: 64px;
}

.sidebar-header {
  padding: 20px 16px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
}

.sidebar-logo {
  display: flex;
  align-items: center;
  gap: 12px;
}

.sidebar-logo svg {
  width: 32px;
  height: 32px;
  color: #667eea;
  flex-shrink: 0;
}

.logo-text {
  font-size: 20px;
  font-weight: 700;
  color: white;
  letter-spacing: 2px;
}

.sidebar-nav {
  flex: 1;
  padding: 16px 8px;
  overflow-y: auto;
}

.nav-section {
  margin-bottom: 24px;
}

.nav-section-title {
  font-size: 11px;
  color: rgba(255, 255, 255, 0.35);
  text-transform: uppercase;
  letter-spacing: 1px;
  padding: 0 12px;
  margin-bottom: 8px;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px;
  border-radius: 10px;
  color: rgba(255, 255, 255, 0.65);
  cursor: pointer;
  transition: all 0.2s;
  font-size: 14px;
  margin-bottom: 2px;
}

.nav-item:hover {
  background: rgba(255, 255, 255, 0.08);
  color: white;
}

.nav-item.active {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.nav-item .el-icon {
  font-size: 18px;
  flex-shrink: 0;
}

.sidebar-footer {
  padding: 16px 8px;
  border-top: 1px solid rgba(255, 255, 255, 0.08);
}

/* 主内容区 */
.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

/* 顶部导航 */
.top-header {
  height: 64px;
  background: white;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 24px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.04);
  z-index: 10;
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
  transition: color 0.2s;
}

.collapse-btn:hover {
  color: #333;
}

.breadcrumb {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  color: #999;
}

.breadcrumb span:last-child {
  color: #333;
  font-weight: 500;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 20px;
}

.header-icon-btn {
  position: relative;
  cursor: pointer;
  color: #666;
  font-size: 20px;
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
  gap: 10px;
  cursor: pointer;
  padding: 6px 12px;
  border-radius: 10px;
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

/* 内容区域 */
.content-area {
  flex: 1;
  padding: 24px;
  overflow-y: auto;
}

/* 欢迎区域 */
.welcome-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.welcome-text h1 {
  font-size: 24px;
  font-weight: 700;
  color: #1a1a1a;
  margin: 0 0 6px;
}

.welcome-text p {
  font-size: 14px;
  color: #888;
  margin: 0;
}

/* 统计卡片 */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
  margin-bottom: 24px;
}

.stat-card {
  background: white;
  border-radius: 16px;
  padding: 24px;
  display: flex;
  align-items: center;
  gap: 20px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.04);
  transition: transform 0.2s, box-shadow 0.2s;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.08);
}

.stat-icon {
  width: 56px;
  height: 56px;
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  flex-shrink: 0;
}

.stat-info {
  flex: 1;
}

.stat-value {
  font-size: 28px;
  font-weight: 700;
  color: #1a1a1a;
  line-height: 1.2;
}

.stat-title {
  font-size: 13px;
  color: #999;
  margin-top: 4px;
}

.stat-trend {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 12px;
  margin-top: 6px;
}

.stat-trend.up {
  color: #67c23a;
}

.stat-trend.down {
  color: #f56c6c;
}

/* 仪表盘网格 */
.dashboard-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
  margin-bottom: 24px;
}

.dashboard-card {
  background: white;
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.04);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.card-header h3 {
  font-size: 16px;
  font-weight: 600;
  color: #1a1a1a;
  margin: 0;
}

/* 图表 */
.chart-placeholder {
  height: 200px;
  display: flex;
  align-items: flex-end;
  padding: 0 10px;
}

.chart-bars {
  display: flex;
  justify-content: space-around;
  align-items: flex-end;
  width: 100%;
  height: 100%;
  gap: 12px;
}

.chart-bar {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: flex-end;
  height: 100%;
}

.bar-fill {
  width: 100%;
  max-width: 40px;
  background: linear-gradient(180deg, #667eea 0%, #764ba2 100%);
  border-radius: 6px 6px 0 0;
  transition: height 0.6s ease;
  min-height: 4px;
}

.bar-label {
  font-size: 12px;
  color: #999;
  margin-top: 8px;
}

/* 活动列表 */
.activity-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.activity-item {
  display: flex;
  gap: 12px;
  align-items: flex-start;
}

.activity-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  margin-top: 6px;
  flex-shrink: 0;
}

.activity-content {
  flex: 1;
}

.activity-text {
  font-size: 14px;
  color: #333;
  margin-bottom: 4px;
}

.activity-time {
  font-size: 12px;
  color: #bbb;
}

/* 快捷操作 */
.quick-actions {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
}

.action-btn {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  padding: 20px 12px;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.2s;
  background: #f8f9fa;
}

.action-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
}

.action-icon {
  width: 44px;
  height: 44px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
}

.action-label {
  font-size: 13px;
  color: #555;
  font-weight: 500;
}

/* 仓库状态 */
.warehouse-status {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.status-item {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.status-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.status-name {
  font-size: 14px;
  color: #333;
  font-weight: 500;
}

.status-percent {
  font-size: 14px;
  font-weight: 600;
  color: #333;
}

/* 响应式 */
@media (max-width: 1200px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  .dashboard-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .sidebar {
    display: none;
  }
  .stats-grid {
    grid-template-columns: 1fr;
  }
  .quick-actions {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
