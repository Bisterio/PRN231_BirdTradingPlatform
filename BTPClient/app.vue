<script setup lang="ts">
useHead({
  title: 'BTP Bird Trading Platform',
  meta: [
    { charset: 'UTF-8' },
    { name: 'viewport', content: 'width=device-width, initial-scale=1.0, minimum-scale=1.0' },
    { name: 'keywords', content: 'Bird trading platform for multi vendors ecommerce' },
    { name: 'description', content: 'BTP is powerful marketplace &amp; ecommerce website.' },
    { name: 'author', content: 'Foie Gras' },
  ],
  link: [
    { rel: 'icon', type: 'image/png', href: 'assets/images/icons/favicon.png' },
    { rel: 'preload', as: 'font', type: 'font/woff2', href: 'assets/vendor/fontawesome-free/webfonts/fa-regular-400.woff2', crossorigin: 'anonymous' },
    { rel: 'preload', as: 'font', type: 'font/woff2', href: 'assets/vendor/fontawesome-free/webfonts/fa-solid-900.woff2', crossorigin: 'anonymous' },
    { rel: 'preload', as: 'font', type: 'font/woff2', href: 'assets/vendor/fontawesome-free/webfonts/fa-brands-400.woff2', crossorigin: 'anonymous' },
    { rel: 'preload', as: 'font', type: 'font/woff2', href: 'assets/fonts/wolmart87d5.woff?png09e', crossorigin: 'anonymous' },
  ],
  bodyAttrs: {
    class: 'test'
  },
  script: [ { innerHTML: 'console.log(\'Hello world\')' } ]
})
</script>
<template>
  <div>
    <NuxtPage />
  </div>
</template>