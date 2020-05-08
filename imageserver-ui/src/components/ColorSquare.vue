<template>
  <div class="row items-center">
    <div :style="{
      width: '1rem',
      height: '1rem',
      border: '1px solid ' + ($q.dark.isActive ? 'white' : 'black'),
      backgroundColor: '#' + value,
      marginRight: '5px'
    }"></div>
    <div>#{{value}}</div>
  </div>
</template>

<script>
export default {
  name: 'ColorSquare',
  props: {
    value: {
      type: String,
      required: true
    }
  }
}
</script>