<script lang="ts">
	import * as Dialog from "$lib/components/ui/dialog/index.js";
	export let field: {
		value: any;
		id: string;
	};
</script>

<Dialog.Root>
	<Dialog.Trigger>
		<img src={`/api/analysis-fields/${field?.id}`} alt="Analyzer output"/>
	</Dialog.Trigger>
	<Dialog.Content class="min-w-[1080px] p-10 bg-white flex justify-center align-center">
		<img src={`/api/analysis-fields/${field?.id}`} alt="Analyzer output"/>
	</Dialog.Content>
</Dialog.Root>