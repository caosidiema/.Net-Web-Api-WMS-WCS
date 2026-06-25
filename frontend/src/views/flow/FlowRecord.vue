<template>
  <div class="page-container">
    <div class="page-header">
      <h2>出入库记录</h2>
      <p>查看所有出入库操作的历史记录</p>
    </div>
    <div class="page-content">
      <el-empty description="出入库记录功能开发中..." />
    </div>
  </div>
</template>

<script setup>
</script>

<style scoped>
.page-container { padding: 24px; }
.page-header { margin-bottom: 24px; }
.page-header h2 { font-size: 20px; font-weight: 600; color: #1a1a1a; margin: 0 0 8px; }
.page-header p { font-size: 14px; color: #888; margin: 0; }
.page-content { background: white; border-radius: 16px; padding: 48px; box-shadow: 0 1px 3px rgba(0,0,0,0.04); }
</style>
