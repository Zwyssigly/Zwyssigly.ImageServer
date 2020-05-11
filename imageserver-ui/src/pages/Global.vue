<template>
  <q-page class="q-pa-md">
    <div class="q-my-md row items-baseline">
      <div class="text-h5 col-auto q-mr-sm">Security</div>
      <div class="text-caption col-auto">(applies to all galleries)</div>
    </div>
    <SecurityForm></SecurityForm>
  </q-page>
</template>

<script>
import SecurityForm from 'components/SecurityForm';

export default {
  name: 'GlobalPage',
  components: { SecurityForm }
}
</script>