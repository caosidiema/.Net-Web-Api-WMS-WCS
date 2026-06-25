<template>
  <div>
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
</template>

<script setup>
import { ref, computed } from 'vue'
import { ElMessage } from 'element-plus'
import {
  Top, Bottom, Plus, Download, Upload, Search, Refresh
} from '@element-plus/icons-vue'
import { useAuthStore } from '../stores/auth'
import { getProfile } from '../api/auth'

const authStore = useAuthStore()
const loading = ref(false)
const chartPeriod = ref('week')

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
    await getProfile()
    ElMessage.success('数据已刷新')
  } catch (err) {
    ElMessage.error('刷新失败，请检查 Token')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
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

.stat-info { flex: 1; }

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

.stat-trend.up { color: #67c23a; }
.stat-trend.down { color: #f56c6c; }

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

.activity-content { flex: 1; }

.activity-text {
  font-size: 14px;
  color: #333;
  margin-bottom: 4px;
}

.activity-time {
  font-size: 12px;
  color: #bbb;
}

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

@media (max-width: 1200px) {
  .stats-grid { grid-template-columns: repeat(2, 1fr); }
  .dashboard-grid { grid-template-columns: 1fr; }
}

@media (max-width: 768px) {
  .stats-grid { grid-template-columns: 1fr; }
  .quick-actions { grid-template-columns: repeat(2, 1fr); }
}
</style>
