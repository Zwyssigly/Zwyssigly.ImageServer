<template>
  <div>
    <q-input 
      label="Tag"
      :value="value.tag"
      @input="val => emitValue({ tag: val })"
    />
    <q-toggle
      label="Crop image"
      :value="value.crop ? true : false"
      @input="updateCrop"
    />    
    <q-input 
      v-if="value.crop"
      label="Aspect ratio"
      :value="value.crop.aspectRatio"
      :rules="[val => /^[1-9][0-9]*\:[1-9][0-9]*$/g.exec(val) ? null : 'Aspect ratio expected']"
      @input="val => emitValue({ crop: { ...value.crop, aspectRatio: val } })"
    />
    <q-select
      v-if="value.crop"
      label="Strategy"
      :value="value.crop.cropStrategy"
      :options="['Cover', 'Contain', 'Stretch']"
      :rules="[val => val.length > 0 ? null : 'Crop strategy expected']"
      @input="val => emitValue({ crop: { ...value.crop, cropStrategy: val, color: null } })"
    />
    <q-input
      label="Background color"
      v-if="value.crop && value.crop.cropStrategy === 'Contain'"      
      :value="value.crop.color ? '#' + value.crop.color : ''"
      clearable
      @input="val => emitValue({ crop: { ...value.crop, color: null } })"
    >
      <template v-slot:append>
        <q-icon name="colorize" class="cursor-pointer">
          <q-popup-proxy transition-show="scale" transition-hide="scale">
            <q-color 
              :value="'#' + value.crop.color"
              @input="val => emitValue({ crop: { ...value.crop, color: val.substr(1) } })"
            />
          </q-popup-proxy>
        </q-icon>
      </template>
    </q-input>
    <q-input
      label="Max width"
      clearable
      :value="value.maxWidth"
      type="number"
      min="1"
      step="1"
      @input="val => emitValue({ maxWidth: parseInt(val) })"
    />
    <q-input
      label="Max height"
      clearable
      :value="value.maxHeight"
      type="number"
      min="1"
      step="1"
      @input="val => emitValue({ maxHeight: parseInt(val) })"
    />
    <q-select
      label="Format"
      :value="value.format"
      emit-value
      :display-value="formats.find(v => v.value === value.format).label"
      :options="formats"
      @input="val => emitValue({ format: val })"
    />    
    <q-field 
      label="Quality"
      :value="value.quality"
      @input="val => emitValue({ quality: val })"
    >
      <q-slider
        :min="0.05"
        :max="1"
        :step="0.05"
        v-model="value.quality"
        color="negative"
        label
      />
    </q-field>
  </div>
</template>

<script>
export default {
  name: 'SizeForm',
  props: {
    value: {
      type: Object,
      default: () => ({
        quality: 0.75,
        format: 'jpg'
      })
    }
  },
  data () {
    return {
      formats: [
        { label: "JPEG", value: 'jpg' },
        { label: "PNG", value: 'png' }
      ]
    }
  },
  methods: {
    updateCrop(val) {
      this.emitValue({ crop: val ? { cropStrategy: 'Cover', aspectRatio: '4:3', color: null } : null });
    },
    emitValue(val) {
      this.$emit('input', { ...this.$props.value, ...val });
    }
  }
}
</script>