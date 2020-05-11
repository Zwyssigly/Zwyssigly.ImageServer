<template>
  <div>
    <q-select
      label="Type"
      :value="value.type"
      :options="['Anonymous', 'Basic']"
      :rules="[val => val.length > 0 ? null : 'Type expected']"
      @input="val => updateType(val)"
    />
    <q-input 
      label="Name"
      :value="value.name"
      :rules="[val => val.length > 0 ? null : 'Name expected']"
      @input="val => emitValue({ name: val })"
      :disable="value.type==='Anonymous'"
    />
    <PasswordInput 
      :value="value.password"
      @input="val => emitValue({ password: val })"
      :disable="value.type==='Anonymous'"
    />
    <div class="text-label q-mt-sm">Permissions: </div>
    <q-option-group
      :value="value.permissions"
      @input="val => emitValue({ permissions: val })"
      :options="permissions"
      color="primary"
      type="checkbox"
    />
  </div>
</template>

<script>
import PasswordInput from 'components/PasswordInput';

export default {
  name: 'AccountForm',
  components: { PasswordInput },
  props: {
    value: {
      type: Object,
      required: true
    }
  },
  data () {
    return {
      permissions: [
        { label: "thumbnail:read", value: 'thumbnail:read' },
        { label: "image:read", value: 'image:read' },
        { label: "image:write", value: 'image:write' },
        { label: "configuration:read", value: 'configuration:read' },
        { label: "configuration:write", value: 'configuration:write' },
        { label: "gallery", value: 'gallery' },
        { label: "security", value: 'security' },
      ]
    }
  },
  methods: {
    updateType(val) {
      switch (val)
      {
        case 'Basic': 
          this.emitValue({ type: 'Basic' });
          break;
        case 'Anonymous':
          this.emitValue({ type: 'Anonymous', name: 'Anonymous', password: null });
          break;
      }
      
    },
    emitValue(val) {
      console.log(val);
      this.$emit('input', { ...this.$props.value, ...val });
    }
  }
}
</script>