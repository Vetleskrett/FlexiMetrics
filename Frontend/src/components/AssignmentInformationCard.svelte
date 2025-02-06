<script lang="ts">
	import { Role, type Assignment, type GradingType } from 'src/types.js';
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';

	export let userRole: Role;
	export let assignment: Assignment;

	const toFriendlyName = (gradingType: GradingType) => {
		switch (gradingType) {
			case 'NoGrading':
				return 'No Grading';
			case 'ApprovalGrading':
				return 'Approval';
			case 'LetterGrading':
				return 'Letter';
			case 'PointsGrading':
				return 'Points';
		}
	};

	$: rows = [
		{
			label: 'Due',
			value: new Date(assignment?.dueDate).toLocaleDateString()
		},
		{
			label: 'Collaboration',
			value: assignment?.collaborationType
		},
		{
			label: 'Mandatory/Optional',
			value: assignment?.mandatory ? 'Mandatory' : 'Optional'
		},
		{
			label: 'Grading',
			value: toFriendlyName(assignment?.gradingType)
		},
		assignment?.gradingType == 'PointsGrading'
			? {
					label: 'Max Points',
					value: assignment?.maxPoints?.toString() || ''
				}
			: undefined,
		userRole == Role.Teacher
			? {
					label: 'Published',
					value: assignment?.published ? 'Yes' : 'No'
				}
			: undefined
	].filter((x) => x != undefined);
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img
				width="40"
				height="40"
				src="https://img.icons8.com/fluency/480/info.png"
				alt="knowledge-sharing"
			/>
			<Card.Title class="m-0 ml-2 text-2xl">Information</Card.Title>
		</div>
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
			{#each rows as row}
				<Separator class="w-full" />
				<div class="flex items-center justify-between px-6 py-4">
					<h1 class="font-semibold">{row.label}</h1>
					<h1>{row.value}</h1>
				</div>
			{/each}
		</div>
	</Card.Content>
</Card.Root>
