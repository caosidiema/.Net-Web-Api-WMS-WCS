<template>
  <div class="login-container">
    <!-- 背景动画元素 -->
    <div class="bg-animation">
      <div class="bg-shape shape-1"></div>
      <div class="bg-shape shape-2"></div>
      <div class="bg-shape shape-3"></div>
    </div>

    <!-- 左侧品牌展示区 -->
    <div class="login-left">
      <div class="brand-content">
        <div class="brand-logo">
          <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M20 7L12 3L4 7M20 7V17L12 21M20 7L12 11M12 11L4 7M12 11V21M4 7V17L12 21" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        </div>
        <h1 class="brand-title">WMS</h1>
        <p class="brand-subtitle">智能仓储管理系统</p>
        <p class="brand-desc">高效 · 智能 · 安全</p>
        <div class="feature-list">
          <div class="feature-item">
            <span class="feature-icon"></span>
            <span>库存管理</span>
          </div>
          <div class="feature-item">
            <span class="feature-icon">🚚</span>
            <span>物流追踪</span>
          </div>
          <div class="feature-item">
            <span class="feature-icon">📊</span>
            <span>数据分析</span>
          </div>
        </div>
      </div>
    </div>

    <!-- 右侧登录表单区 -->
    <div class="login-right">
      <div class="login-card">
        <div class="login-header">
          <h2 class="login-title">欢迎回来</h2>
          <p class="login-subtitle">请登录您的账户</p>
        </div>

        <el-form ref="formRef" :model="form" :rules="rules" label-width="0" class="login-form">
          <el-form-item prop="userName">
            <div class="input-wrapper">
              <el-icon class="input-icon"><User /></el-icon>
              <el-input
                v-model="form.userName"
                placeholder="请输入用户名"
                size="large"
                class="custom-input"
              />
            </div>
          </el-form-item>

          <el-form-item prop="password">
            <div class="input-wrapper">
              <el-icon class="input-icon"><Lock /></el-icon>
              <el-input
                v-model="form.password"
                type="password"
                placeholder="请输入密码"
                size="large"
                show-password
                class="custom-input"
                @keyup.enter="handleLogin"
              />
            </div>
          </el-form-item>

          <div class="form-options">
            <label class="remember-me">
              <el-checkbox v-model="rememberMe">记住我</el-checkbox>
            </label>
            <a class="forgot-link" href="#">忘记密码？</a>
          </div>

          <el-form-item>
            <el-button
              type="primary"
              size="large"
              :loading="loading"
              @click="handleLogin"
              class="login-btn"
            >
              登 录
            </el-button>
          </el-form-item>
        </el-form>

        <div class="test-account">
          <el-icon><InfoFilled /></el-icon>
          <span>测试账号：admin / 123456</span>
        </div>

        <div class="login-footer">
          <span>© 2024 WMS 智能仓储管理系统</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { User, Lock, InfoFilled } from '@element-plus/icons-vue'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const authStore = useAuthStore()
const formRef = ref(null)
const loading = ref(false)
const rememberMe = ref(false)

const form = reactive({
  userName: '',
  password: ''
})

const rules = {
  userName: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }]
}

async function handleLogin() {
  await formRef.value.validate()
  loading.value = true
  try {
    await authStore.login(form)
    ElMessage.success('登录成功')
    router.push('/home')
  } catch (err) {
    ElMessage.error(err.response?.data?.message || '登录失败')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-container {
  display: flex;
  height: 100vh;
  overflow: hidden;
  position: relative;
}

/* 背景动画 */
.bg-animation {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  overflow: hidden;
  z-index: 0;
}

.bg-shape {
  position: absolute;
  border-radius: 50%;
  opacity: 0.1;
  animation: float 20s infinite ease-in-out;
}

.shape-1 {
  width: 400px;
  height: 400px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  top: -100px;
  right: -100px;
  animation-delay: 0s;
}

.shape-2 {
  width: 300px;
  height: 300px;
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
  bottom: -50px;
  left: -50px;
  animation-delay: -5s;
}

.shape-3 {
  width: 200px;
  height: 200px;
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  top: 50%;
  left: 50%;
  animation-delay: -10s;
}

@keyframes float {
  0%, 100% { transform: translate(0, 0) scale(1); }
  33% { transform: translate(30px, -30px) scale(1.1); }
  66% { transform: translate(-20px, 20px) scale(0.9); }
}

/* 左侧品牌区 */
.login-left {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%);
  position: relative;
  z-index: 1;
  padding: 60px;
}

.brand-content {
  text-align: center;
  color: white;
}

.brand-logo {
  width: 80px;
  height: 80px;
  margin: 0 auto 30px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.brand-logo svg {
  width: 50px;
  height: 50px;
  color: white;
}

.brand-title {
  font-size: 48px;
  font-weight: 700;
  margin: 0 0 10px;
  letter-spacing: 8px;
}

.brand-subtitle {
  font-size: 20px;
  opacity: 0.9;
  margin: 0 0 10px;
}

.brand-desc {
  font-size: 14px;
  opacity: 0.7;
  margin: 0 0 40px;
  letter-spacing: 4px;
}

.feature-list {
  display: flex;
  gap: 30px;
  justify-content: center;
}

.feature-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  opacity: 0.9;
}

.feature-icon {
  font-size: 28px;
}

/* 右侧登录区 */
.login-right {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #ffffff;
  position: relative;
  z-index: 1;
  padding: 60px;
}

.login-card {
  width: 100%;
  max-width: 420px;
}

.login-header {
  margin-bottom: 40px;
}

.login-title {
  font-size: 32px;
  font-weight: 700;
  color: #1a1a1a;
  margin: 0 0 8px;
}

.login-subtitle {
  font-size: 16px;
  color: #666;
  margin: 0;
}

.login-form {
  margin-bottom: 30px;
}

.input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.input-icon {
  position: absolute;
  left: 16px;
  color: #999;
  font-size: 18px;
  z-index: 1;
}

.custom-input :deep(.el-input__wrapper) {
  padding-left: 48px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
  transition: all 0.3s;
}

.custom-input :deep(.el-input__wrapper:hover) {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
}

.custom-input :deep(.el-input__wrapper.is-focus) {
  box-shadow: 0 4px 16px rgba(64, 158, 255, 0.2);
}

.form-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.remember-me {
  font-size: 14px;
  color: #666;
}

.forgot-link {
  font-size: 14px;
  color: #409eff;
  text-decoration: none;
  transition: color 0.3s;
}

.forgot-link:hover {
  color: #66b1ff;
}

.login-btn {
  width: 100%;
  height: 50px;
  font-size: 16px;
  font-weight: 600;
  border-radius: 12px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border: none;
  transition: all 0.3s;
}

.login-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(102, 126, 234, 0.4);
}

.login-btn:active {
  transform: translateY(0);
}

.test-account {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 16px;
  background: #f8f9fa;
  border-radius: 8px;
  font-size: 13px;
  color: #666;
  margin-bottom: 20px;
}

.test-account .el-icon {
  color: #409eff;
}

.login-footer {
  text-align: center;
  font-size: 12px;
  color: #999;
}

/* 响应式 */
@media (max-width: 968px) {
  .login-left {
    display: none;
  }

  .login-right {
    flex: 1;
  }
}
</style>
