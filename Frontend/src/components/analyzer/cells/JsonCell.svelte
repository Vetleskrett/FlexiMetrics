<script lang="ts">
	export let field: {
		value: any;
	};
</script>

<div class="rounded bg-[#F2F2F2] p-2 font-mono">
	<p style="white-space: pre-wrap">{field?.value}</p>
</div>
