<script lang="ts">
	import * as Card from '$lib/components/ui/card';
	import type { Analyzer } from 'src/types/';
	import CustomButton from '../CustomButton.svelte';
	import Save from 'lucide-svelte/icons/save';
	import Undo_2 from 'lucide-svelte/icons/undo-2';
	import { Input } from '$lib/components/ui/input';
	import { goto } from '$app/navigation';
	import * as Form from 'src/lib/components/ui/form';
	import { superForm } from 'sveltekit-superforms';
	import FileUpload from '../inputs/FileUpload.svelte';
	import { transformErrors } from 'src/utils';
	import { ArrowDownToLine } from 'lucide-svelte';
	import { postAnalyzer, postAnalyzerScript, putAnalyzer } from 'src/api';

	export let courseId: string;
	export let assignmentId: string;
	export let edit: boolean;
	export let analyzer: Analyzer | undefined = undefined;
	export let script: string | undefined = undefined;

	type AnalyzerFormData = {
		name: string;
		script?: File;
	};

	let analyzerFormData: AnalyzerFormData = {
		name: '',
		script: undefined
	};

	if (edit) {
		analyzerFormData = {
			name: analyzer!.name,
			script: new File([new Blob([script!], { type: 'plain/text' })], analyzer!.fileName)
		};
	}

	const onSubmitEdit = async () => {
		return putAnalyzer(analyzer!.id, {
			name: analyzerFormData.name,
			fileName: analyzerFormData.script?.name ?? ''
		});
	};

	const onSubmitCreate = async () => {
		return postAnalyzer({
			name: analyzerFormData.name,
			fileName: analyzerFormData.script?.name ?? '',
			assignmentId: assignmentId
		});
	};

	const onSubmit = async (formEvent: any) => {
		formEvent.cancel();
		var promise = edit ? onSubmitEdit() : onSubmitCreate();

		promise
			.then(async (response) => {
				var analyzer = response.data;

				var formData = new FormData();
				formData.append('script', analyzerFormData.script!);
				await postAnalyzerScript(analyzer.id, formData);

				goto(`/teacher/courses/${courseId}/assignments/${assignmentId}/analyzers/${analyzer.id}`, {
					invalidateAll: true
				});
			})
			.catch((exception) => {
				if (exception?.response?.data?.errors) {
					const validationErrors = transformErrors(exception.response.data.errors);
					console.error(validationErrors);
					errors.set(validationErrors);
				} else {
					console.error(exception);
				}
			});
	};

	const form = superForm(analyzerFormData, {
		onSubmit: onSubmit,
		dataType: 'json'
	});
	const { enhance, errors } = form;
</script>

<div class="flex flex-row gap-8">
	<form method="post" use:enhance>
		<Card.Root class="w-[600px] overflow-hidden p-0">
			<Card.Header class="mb-6 flex flex-row items-center justify-between">
				<div class="flex items-center">
					<img
						width="48"
						height="48"
						src="https://img.icons8.com/fluency/480/artificial-intelligence--v2.png"
						alt="knowledge-sharing"
					/>
					<Card.Title class="ml-4 text-3xl">{edit ? 'Edit Analyzer' : 'New Analyzer'}</Card.Title>
				</div>
			</Card.Header>
			<Card.Content class="flex flex-col gap-4 px-6 py-0">
				<Form.Field {form} name="name">
					<Form.Control let:attrs>
						<Form.Label for="name">Name</Form.Label>
						<Input {...attrs} id="name" bind:value={analyzerFormData.name} />
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>
				<Form.Field {form} name="script">
					<Form.Control let:attrs>
						<Form.Label for="script">Script</Form.Label>
						<FileUpload
							{...attrs}
							bind:file={analyzerFormData.script}
							accept=".py"
							on:change={() => {
								if (analyzerFormData.script) {
									analyzerFormData.script?.text().then((text) => (script = text));
								} else {
									script = undefined;
								}
							}}
						/>
					</Form.Control>
					<Form.FieldErrors />
				</Form.Field>
			</Card.Content>
			<Card.Footer class="flex flex-col items-center p-0">
				<div class="m-4 flex gap-4">
					<CustomButton color="yellow" href={edit ? './' : '../'}>
						<Undo_2 size="20" />
						<p>Return</p>
					</CustomButton>
					<CustomButton color="submit">
						<Save size="20" />
						<p>{edit ? 'Edit' : 'Create'}</p>
					</CustomButton>
				</div>
			</Card.Footer>
		</Card.Root>
	</form>

	<Card.Root class="h-min w-[500px] p-0">
		<Card.Header class="m-0 flex flex-row items-center justify-between px-6">
			<div class="flex items-center">
				<img
					width="48"
					height="48"
					src="https://img.icons8.com/fluency/480/code-file.png"
					alt="knowledge-sharing"
				/>
				<Card.Title class="ml-4 text-3xl">Script</Card.Title>
			</div>
			{#if script}
				<a
					class="mx-2 hover:text-blue-500"
					download={analyzerFormData.script?.name}
					href={'data:text/plain;charset=utf-8,' + encodeURIComponent(script)}
				>
					<ArrowDownToLine size="30" />
				</a>
			{/if}
		</Card.Header>
		<Card.Content class="m-0 px-6 pb-6 pt-4">
			{#if script}
				<h1 class="font-semibold">{analyzerFormData.script?.name}</h1>
				<div class="w-full rounded bg-gray-100 p-2 font-mono">
					<p style="white-space: pre-wrap">{script}</p>
				</div>
			{:else}
				<p class="text-center">No script uploaded</p>
			{/if}
		</Card.Content>
	</Card.Root>
</div>
