<script lang="ts">
	import * as Card from '$lib/components/ui/card';
	import Plus from 'lucide-svelte/icons/plus';
	import Button from '$lib/components/ui/button/button.svelte';
	import { Textarea } from 'src/lib/components/ui/textarea';
	import CustomButton from '../CustomButton.svelte';

	export let addFunction: (input: string, file: File | null) => void;
	let studentInput: string;
	let file: File | null;
	let fileInput: HTMLInputElement;

	function handleFileUpload(event: Event): void {
		const input = event.target as HTMLInputElement;
		file = input.files ? input.files[0] : null;
		file?.text().then((text) => (studentInput = text));
	}

	function openFileExplorer() {
		fileInput.click();
	}
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img width="48" height="48" src="https://img.icons8.com/fluency/480/csv.png" alt="csv" />
			<Card.Title class="ml-4 text-3xl">Add Students</Card.Title>
		</div>
	</Card.Header>
	<Card.Content class="p-5">
		<div class="flex flex-col items-center gap-2">
			<div class="flex w-full flex-col gap-4">
				<div>
					<p>Required format:</p>
					<b>email1, email2, email3, ...</b>
				</div>

				<div class="flex gap-1">
					<p>Copy and Paste or</p>
					<button on:click={openFileExplorer} class="text-blue-500 hover:text-blue-700">
						Browse Files
					</button>
					<input
						bind:this={fileInput}
						type="file"
						on:change={handleFileUpload}
						style="display: none;"
					/>
				</div>
			</div>

			<Textarea
				class="h-48 border-2 border-dotted border-gray-500"
				placeholder="ola@email.com, kari@email.com, per@email.com, ..."
				bind:value={studentInput}
			/>
			<CustomButton color="green" on:click={async () => addFunction(studentInput, file)}>
				<Plus />
				<p>Add</p>
			</CustomButton>
		</div>
	</Card.Content>
</Card.Root>
