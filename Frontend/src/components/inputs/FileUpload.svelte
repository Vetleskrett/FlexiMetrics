<script lang="ts">
	import { Upload } from 'lucide-svelte';
	import CustomButton from '../CustomButton.svelte';
	import type { FileMetadata } from 'src/types';

	let fileInput: HTMLInputElement;
	let isDragOver = false;

	export let file: File | FileMetadata | undefined = undefined;

	function triggerFileSelect() {
		fileInput.click();
	}

	function onFileChange(event: Event) {
		const target = event.target as HTMLInputElement;
		file = target.files ? target.files[0] : undefined;
	}

	function onDragOver(event: DragEvent) {
		event.preventDefault();
		isDragOver = true;
	}

	function onDragLeave(event: DragEvent) {
		event.preventDefault();
		isDragOver = false;
	}

	function onDrop(event: DragEvent) {
		event.preventDefault();
		isDragOver = false;
		if (event.dataTransfer && event.dataTransfer.files) {
			fileInput.files = event.dataTransfer.files;
			file = event.dataTransfer.files[0];
		}
	}
</script>

<div class="flex flex-col">
	<div
		role="button"
		tabindex="0"
		class={`flex w-full flex-col items-center justify-center rounded border-2 p-8 transition-colors duration-200 ${
			isDragOver ? 'border-blue-400 bg-blue-100' : 'border-dashed border-gray-300 bg-blue-50'
		}`}
		on:dragover={onDragOver}
		on:dragleave={onDragLeave}
		on:drop={onDrop}
	>
		<Upload color="gray" size="32" class="mb-2" />
		<p class="text-gray-600">Drag & Drop file</p>
		<p class="my-2 text-gray-600">or</p>

		<CustomButton color="blue" on:click={triggerFileSelect}>Browse Files</CustomButton>

		<input
			type="file"
			multiple={false}
			bind:this={fileInput}
			class="hidden"
			on:change={onFileChange}
			on:change
		/>
	</div>

	{#if file}
		{#if file instanceof File}
			<p><span class="font-semibold">File:</span> {file.name}</p>
		{:else}
			<p><span class="font-semibold">File:</span> {file.FileName}</p>
		{/if}
	{/if}
</div>
